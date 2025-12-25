using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;
using CommandTypes;
using System.Collections;
using System;
using Vector3 = UnityEngine.Vector3;
using System.Linq;
public class EditorManager : MonoBehaviour, IDataPersistence
{
    // This class manages level loading and object manipulation.
    // 
    // Perhaps there could be separation with level 
    // management and object manipulation in two classes.
    public static EditorManager Instance { get; private set; }

    [SerializeField] private int levelNum = 0;
    [SerializeField] private List<LevelData> playerLevels = new();

    private List<ObjectData> editorPrefabs;
    private ObjectData[] allCurrentSceneObjects;

    [SerializeField] private float objMoveSensitivity = 70f;
    [SerializeField] private float objRotateSensitivity = 30f;
    [SerializeField] private float objScaleSensitivity = 30f;

    private readonly Dictionary<RuntimeTransformGizmo, Coroutine> activeDrags = new();

    private readonly Stack<IEditorCommand> performedCommands = new();
    private readonly Stack<IEditorCommand> undoneCommands = new();

    IEditorCommand activeCommand;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogError("Already an EditorManager Instance in this scene.\nDestroying current instance.");
            Destroy(this);
            return;
        }
        Instance = this;
    }

    private void OnEnable()
    {
        EventManager.Instance.OnCreateNewLevel.AddListener(CreateNewLevel);
        EventManager.Instance.OnGizmoSelected.AddListener(StartTransform);
        EventManager.Instance.OnGizmoDeselected.AddListener(PerformCommand);
        EventManager.Instance.OnGizmoDeselected.AddListener(StopTransform);
    }

    private void OnDisable()
    {
        EventManager.Instance.OnCreateNewLevel.RemoveListener(CreateNewLevel);
        EventManager.Instance.OnGizmoSelected.RemoveListener(StartTransform);
        EventManager.Instance.OnGizmoDeselected.RemoveListener(PerformCommand);
        EventManager.Instance.OnGizmoDeselected.RemoveListener(StopTransform);


    }

    private void Start()
    {
        InitializeLists();
    }
    private void Update()
    {
        //Debug.Log($"mouseDelta tasty: {Input.mousePositionDelta}");
        if (Input.GetKeyDown(KeyCode.Z))
        {
            UndoLastCommand();
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            RedoLastCommand();
        }
    }

    [ContextMenu("Create New Level")]
    public void CreateNewLevel()
    {
        LevelData level = new() { levelID = playerLevels.Count };

        for (int i = 0; i < allCurrentSceneObjects.Length; i++)
        {
            ObjectInfo info = new()
            {
                objID = allCurrentSceneObjects[i].objID,
                position = allCurrentSceneObjects[i].transform.position,
                rotation = allCurrentSceneObjects[i].transform.localRotation,
                scale = allCurrentSceneObjects[i].transform.localScale
            };
            level.objectInfos.Add(info);
        }

        playerLevels.Add(level);
    }
    private void InitializeLists()
    {
        editorPrefabs = EditorObjectPrefabsContainer.Instance.editorPrefabs;
        allCurrentSceneObjects = FindObjectsByType<ObjectData>(FindObjectsSortMode.None);
    }

    [ContextMenu("Load Level")]
    public void LoadLevel()
    {
        LoadLevel(levelNum);
    }

    public void LoadLevel(int levelID)
    {
        if (levelID < 0 || levelID >= playerLevels.Count)
        {
            Debug.LogError("levelID out of range");
            return;
        }

        foreach (ObjectData obj in FindObjectsByType<ObjectData>(FindObjectsSortMode.None))
        {
            obj.gameObject.SetActive(false);
            Destroy(obj.gameObject);
        }

        foreach (ObjectInfo oi in playerLevels[levelID].objectInfos)
        {
            ObjectData newObj = Instantiate(editorPrefabs[oi.objID], oi.position, oi.rotation, GameObject.FindGameObjectWithTag("ObjectContainer").transform);
            newObj.transform.localScale = oi.scale;
            newObj.gameObject.SetActive(true);

            foreach (MonoBehaviour component in newObj.GetComponents<MonoBehaviour>()) component.enabled = true;

            foreach (Collider collider in newObj.GetComponents<Collider>()) collider.enabled = true;

            foreach (Renderer renderer in newObj.GetComponents<Renderer>()) renderer.enabled = true;

            foreach (Outline outline in newObj.GetComponents<Outline>())
            {
                outline.OutlineColor = EditorSelectionScript.GetHoverColor();
                outline.OutlineMode = Outline.Mode.OutlineHidden;
            }
        }
    }

    public void ConstructObjectDataWith(ref ObjectData newObjData, ObjectData oldObjData)
    {
        newObjData.position = oldObjData.position;
        newObjData.scale = oldObjData.scale;
        newObjData.rotation = oldObjData.rotation;
        newObjData.objID = oldObjData.objID;
        newObjData.IsSelected = oldObjData.IsSelected;
        newObjData.connectedGizmos = new Dictionary<RuntimeTransformGizmo.TransformType, RuntimeTransformGizmo>(oldObjData.connectedGizmos);
    }

    public void StartTransform(RuntimeTransformGizmo gizmo)
    {
        if (activeDrags.ContainsKey(gizmo)) return; // already dragging

        RuntimeTransformGizmo.TransformType axisTypeFlags = gizmo.transformType &
        (RuntimeTransformGizmo.TransformType.Linear
        | RuntimeTransformGizmo.TransformType.Rotation
        | RuntimeTransformGizmo.TransformType.Scale);

        bool isPivot = gizmo.Target.objID == 0;
        List<ObjectData> selectedObjects = EditorSelectionScript.Instance.SelectedObjects.ToList<ObjectData>();
        if (isPivot) selectedObjects.Add(gizmo.Target);
        switch (gizmo.transformType & axisTypeFlags)
        {
            case RuntimeTransformGizmo.TransformType.Linear:
                activeCommand = new MoveCommand(selectedObjects);
                break;
            case RuntimeTransformGizmo.TransformType.Rotation:
                activeCommand = new RotateCommand(selectedObjects);
                break;
            case RuntimeTransformGizmo.TransformType.Scale:
                activeCommand = new ScaleCommand(selectedObjects);
                break;
            default:
                break;
        }

        activeDrags.Add(gizmo, StartCoroutine(TransformObject(gizmo)));
    }

    public void StopTransform(RuntimeTransformGizmo gizmo)
    {
        if (!activeDrags.TryGetValue(gizmo, out Coroutine coroutine)) return;
        StopCoroutine(activeDrags[gizmo]);
        activeDrags.Remove(gizmo);
    }

    private IEnumerator TransformObject(RuntimeTransformGizmo gizmo)
    {
        bool isPivot = gizmo.Target.objID == 0;

        switch (EditorSelectionScript.EditorSelectionState)
        {
            case EditorSelectionScript.SelectionState.Linear:
                while (gizmo.IsHeld)
                {
                    Transform gizmoTransform = gizmo.transform;

                    // Project two end points of the up vector
                    // and then subtract to get the connecting 
                    // screen space vector
                    Vector2 gizmoAxisToScreen =
                        (Camera.main.WorldToScreenPoint(gizmoTransform.up + gizmoTransform.position)
                        - Camera.main.WorldToScreenPoint(gizmoTransform.position)).normalized;

                    float distToObj = (Camera.main.transform.position - gizmoTransform.position).magnitude;

                    //Calculate how much the delta is in the direction of the axis
                    float initialMouseDelta = Vector2.Dot(Input.mousePositionDelta, gizmoAxisToScreen);

                    //Gizmo should naturally face "up" in the correct direction based on instantiation
                    Vector3 worldDelta = gizmoTransform.up * initialMouseDelta * distToObj * objMoveSensitivity * Time.deltaTime;
                    foreach (ObjectData selectedObj in EditorSelectionScript.Instance.SelectedObjects)
                        selectedObj.transform.Translate(worldDelta, Space.World);

                    if (isPivot) gizmo.Target.transform.Translate(worldDelta, Space.World);
                    yield return null;
                }
                break;
            case EditorSelectionScript.SelectionState.Rotation:
                while (gizmo.IsHeld)
                {
                    foreach (ObjectData selectedObj in EditorSelectionScript.Instance.SelectedObjects)
                    {
                        Transform targetTransform = selectedObj.transform;
                        Transform gizmoTransform = gizmo.transform;

                        Transform objTransform = gizmo.Target.transform;

                        //Gizmo should naturally face "up" in the correct direction based on instantiation
                        Vector3 rotationAxis = gizmoTransform.up;

                        Vector3 hitPoint = GetMousePlaneIntersection(rotationAxis, objTransform.position, Vector3.zero);
                        Vector3 prevHitPoint = GetMousePlaneIntersection(rotationAxis, objTransform.position, Input.mousePositionDelta);

                        Vector3 currentDir = (hitPoint - objTransform.position).normalized;

                        Vector3 lastDir = (prevHitPoint - objTransform.position).normalized;


                        //Calculate how much the angle needs to change based on mouse position
                        float deltaAngle = Vector3.SignedAngle(lastDir, currentDir, rotationAxis) * objRotateSensitivity * Time.deltaTime;

                        targetTransform.RotateAround(gizmo.Target.transform.position, rotationAxis, deltaAngle);
                    }
                    yield return null;
                }
                break;
            case EditorSelectionScript.SelectionState.Scale:
                while (gizmo.IsHeld)
                {
                    Transform gizmoTransform = gizmo.transform;

                    // Project two end points of the up vector
                    // and then subtract to get the connecting 
                    // screen space vector
                    Vector2 gizmoAxisToScreen =
                        (Camera.main.WorldToScreenPoint(gizmoTransform.up + gizmoTransform.position)
                        - Camera.main.WorldToScreenPoint(gizmoTransform.position)).normalized;

                    float distToObj = (Camera.main.transform.position - gizmoTransform.position).magnitude;

                    //Calculate how much the delta is in the direction of the axis
                    float initialMouseDelta = Vector2.Dot(Input.mousePositionDelta, gizmoAxisToScreen);

                    //Gizmo should naturally face "up" in the correct direction based on instantiation
                    float scaleDelta = initialMouseDelta * distToObj * objScaleSensitivity * 0.001f;
                    float scaleFactor = 1f + scaleDelta; // scaleDelta can be negative or positive
                    if (gizmo.transformType == RuntimeTransformGizmo.TransformType.ScaleXYZ)
                    {
                        foreach (ObjectData selectedObj in EditorSelectionScript.Instance.SelectedObjects)
                        {
                            Transform targetTransform = selectedObj.transform;

                            Vector3 pivotToObj = targetTransform.position - gizmo.Target.transform.position;
                            pivotToObj *= scaleFactor;
                            targetTransform.position = gizmo.Target.transform.position + pivotToObj;

                            // Scale the object uniformly
                            targetTransform.localScale *= scaleFactor;
                        }
                    }
                    else
                    {
                        foreach (ObjectData selectedObj in EditorSelectionScript.Instance.SelectedObjects)
                        {
                            Transform targetTransform = selectedObj.transform;

                            Vector3 pivotToObj = targetTransform.position - gizmo.Target.transform.position;
                            Vector3 axis = gizmo.LocalUp.normalized;

                            // Project pivotToObj onto axis
                            float projection = Vector3.Dot(pivotToObj, axis);
                            Vector3 projected = axis * projection;
                            Vector3 perpendicular = pivotToObj - projected;

                            // Scale along axis
                            Vector3 newPivotToObj = perpendicular + projected * scaleFactor;

                            targetTransform.position = gizmo.Target.transform.position + newPivotToObj;

                            // Scale the object along the same axis
                            Vector3 newScale = targetTransform.localScale + axis * scaleDelta;
                            targetTransform.localScale = Vector3.Max(newScale, Vector3.one * 0.1f);
                        }
                    }
                    yield return null;
                }
                break;
            default:
                break;

        }
    }


    private Vector3 GetMousePlaneIntersection(Vector3 axis, Vector3 pivot, Vector3 delta)
    {
        Plane plane = new Plane(axis, pivot);
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition - delta);

        if (plane.Raycast(ray, out float enter))
            return ray.GetPoint(enter);

        return pivot;
    }
    public void PerformCommand(RuntimeTransformGizmo gizmo)
    {
        Debug.Log("Performed Command");
        //No longer able to redo old commands as now we're performing a new command
        undoneCommands.Clear();


        activeCommand.SetFinalState();
        activeCommand.Execute();

        performedCommands.Push(activeCommand);
    }

    public void UndoLastCommand()
    {
        Debug.Log("Undo tried");
        if (performedCommands.Count == 0)
        {
            EventManager.Instance.OnUndoUnavailable.Invoke();
            return;
        }

        Debug.Log("Undo successful");

        IEditorCommand cmd = performedCommands.Pop();
        cmd.Undo();

        //Add to redo stack
        undoneCommands.Push(cmd);
    }
    public void RedoLastCommand()
    {
        if (undoneCommands.Count == 0)
        {
            EventManager.Instance.OnRedoUnavailable.Invoke();
            return;
        }

        IEditorCommand cmd = undoneCommands.Pop();
        cmd.Execute();

        //Add back to undo stack
        performedCommands.Push(cmd);
    }

    public void SaveData(ref GameData data)
    {
        data.playerLevels = new List<LevelData>(playerLevels);
        data.editorPrefabs = editorPrefabs;
    }

    public void LoadData(GameData data)
    {
        playerLevels = new List<LevelData>(data.playerLevels);
        editorPrefabs = data.editorPrefabs;
    }
}
