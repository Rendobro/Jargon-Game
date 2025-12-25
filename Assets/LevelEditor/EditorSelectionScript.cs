using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;
using System;
using System.Linq;
using System.Collections;
using System.Net.Security;
using UnityEngine.Android;
public class EditorSelectionScript : MonoBehaviour
{
    public static EditorSelectionScript Instance { get; private set; }
    private readonly int editorObjectLayer = 1 << 8;
    private readonly int editorGizmoLayer = 1 << 9;
    private Transform lastHitTransform = null;
    private RuntimeTransformGizmo lastHitGizmo = null;
    [SerializeField] private static Color selectedColor;
    [SerializeField] private static Color hoverColor;
    private static Color mixedColor;
    private bool multiSelectOn => Input.GetButton("MultiSelect");
    private Vector2 initialMouseDragPos = Vector2.zero;
    [SerializeField] private Texture dragBoxTexture;
    private bool startedDrag
    => Input.GetButton("Fire1")
    && Input.mousePositionDelta.magnitude > 0.005f
    && !dragging
    && (lastHitGizmo == null || !lastHitGizmo.IsHeld);
    private bool endedDrag => Input.GetButtonUp("Fire1") && dragging;
    private bool dragging = false;
    private ObjectData pivot;
    private readonly HashSet<ObjectData> selectedObjects = new();
    public IReadOnlyCollection<ObjectData> SelectedObjects => selectedObjects;
    private List<ObjectData> previousSelectedObjects = new();
    private static SelectionState selectionState;
    public static SelectionState EditorSelectionState
    {
        get => selectionState;
        set
        {
            if (selectionState == value) return;

            selectionState = value;
            EventManager.Instance.OnSelectionStateChanged.Invoke();
        }
    }
    public enum SelectionState
    {
        Linear,
        Rotation,
        Scale
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogError("Already an EventManager Instance in this scene.\nDestroying current instance.");
            Destroy(gameObject);
            return;
        }
        Instance = this;

        selectedColor = Color.green;
        hoverColor = Color.cyan;
        float mixAmount = 0.35f;
        Vector3 colorCoords = Vector3.Lerp(new Vector3(hoverColor.r, hoverColor.g, hoverColor.b), new Vector3(selectedColor.r, selectedColor.g, selectedColor.b), mixAmount);
        mixedColor = new Color(colorCoords.x, colorCoords.y, colorCoords.z);
        pivot = Instantiate(EditorObjectPrefabsContainer.Instance.editorPrefabs[0], Vector3.zero, Quaternion.identity);
        pivot.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        EventManager.Instance.OnObjectSelected.AddListener(SetTransformGizmos);
        EventManager.Instance.OnGizmoSelected.AddListener(SetSelectedGizmoColor);
        EventManager.Instance.OnObjectDeselected.AddListener(RemoveTransformGizmos);
        EventManager.Instance.OnSelectionStateChanged.AddListener(AlterTransformGizmos);
    }

    private void OnDisable()
    {
        EventManager.Instance.OnObjectSelected.RemoveListener(SetTransformGizmos);
        EventManager.Instance.OnGizmoSelected.RemoveListener(SetSelectedGizmoColor);
        EventManager.Instance.OnObjectDeselected.RemoveListener(RemoveTransformGizmos);
        EventManager.Instance.OnSelectionStateChanged.RemoveListener(AlterTransformGizmos);
    }

    void Update()
    {
        HandleGizmoSelecting();
        HandleObjectSelecting();
        HandleMultiSelectPivot();
        HandleDragSelecting();
        if (startedDrag) dragging = true;
        if (endedDrag) dragging = false;

        // Temporary testing of changing states
        if (Input.GetKeyDown(KeyCode.J)) EditorSelectionState = SelectionState.Linear;
        if (Input.GetKeyDown(KeyCode.K)) EditorSelectionState = SelectionState.Rotation;
        if (Input.GetKeyDown(KeyCode.L)) EditorSelectionState = SelectionState.Scale;
    }

    private void OnGUI()
    {
        if (dragging)
        {
            GUI.Box(new Rect
            (initialMouseDragPos.x,
            Screen.height - initialMouseDragPos.y,
            Input.mousePosition.x - initialMouseDragPos.x,
            initialMouseDragPos.y - Input.mousePosition.y),
            dragBoxTexture);
        }
    }

    private void HandleMultiSelectPivot()
    {
        if (selectedObjects.Count > 1)
        {
            Vector3 pivotPos = FindObjectsMidpoint(selectedObjects.ToArray<ObjectData>());
            pivot.transform.position = pivotPos;
            foreach (ObjectData obj in selectedObjects)
                RemoveTransformGizmos(obj);
            SetTransformGizmos(pivot);
            pivot.gameObject.SetActive(true);
        }
    }

    private void HandleDragSelecting()
    {
        if (startedDrag)
        {
            initialMouseDragPos = Input.mousePosition;
            Debug.Log(initialMouseDragPos);
        }
        if (endedDrag)
        {
            Vector2 finalMousePos = Input.mousePosition;
            Rect enclosingRect = new Rect(initialMouseDragPos.x, initialMouseDragPos.y, finalMousePos.x - initialMouseDragPos.x, finalMousePos.y - initialMouseDragPos.y);
            float xMin = Mathf.Min(initialMouseDragPos.x, finalMousePos.x);
            float xMax = Mathf.Max(initialMouseDragPos.x, finalMousePos.x);
            float yMin = Mathf.Min(initialMouseDragPos.y, finalMousePos.y);
            float yMax = Mathf.Max(initialMouseDragPos.y, finalMousePos.y);

            Camera cam = Camera.main;
            Ray r00 = cam.ScreenPointToRay(new Vector2(xMin, yMin));
            Ray r10 = cam.ScreenPointToRay(new Vector2(xMax, yMin));
            Ray r11 = cam.ScreenPointToRay(new Vector2(xMax, yMax));
            Ray r01 = cam.ScreenPointToRay(new Vector2(xMin, yMax));
            Vector3 O = cam.transform.position;

            Plane left = new Plane(O, r01.direction + O, r00.direction + O);
            Plane right = new Plane(O, r10.direction + O, r11.direction + O);
            Plane bottom = new Plane(O, r00.direction + O, r10.direction + O);
            Plane top = new Plane(O, r11.direction + O, r01.direction + O);
            Plane near = new Plane(cam.transform.forward, cam.transform.position + cam.nearClipPlane * cam.transform.forward);
            Plane far = new Plane(-cam.transform.forward, cam.transform.position + cam.transform.forward * cam.farClipPlane);
            Plane[] planes = new Plane[] { left, right, bottom, top, near, far };

            Debug.DrawRay(cam.transform.position, r00.direction * 100, Color.red);
            Debug.DrawRay(cam.transform.position, r10.direction * 100, Color.green);
            Debug.DrawRay(cam.transform.position, r11.direction * 100, Color.blue);
            Debug.DrawRay(cam.transform.position, r01.direction * 100, Color.yellow);

            // remember to cache a list of all current scene objects
            ObjectData[] allSceneObjects = FindObjectsByType<ObjectData>(FindObjectsSortMode.None);

            foreach (ObjectData obj in allSceneObjects)
            {
                if (obj.objID == 0) continue;
                if (!obj.TryGetComponent(out Collider col)) continue;

                if (GeometryUtility.TestPlanesAABB(planes, col.bounds))
                {
                    SelectObject(obj);
                }
            }
        }
    }


    private void HandleObjectSelecting()
    {
        if (lastHitGizmo != null && lastHitGizmo.IsHeld) return;
        bool isObjectHit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hitInfo, 20000f, editorObjectLayer);
        if (isObjectHit)
        {
            Transform currentTransform = hitInfo.transform;

            if (lastHitTransform != null && !currentTransform.Equals(lastHitTransform) && !lastHitTransform.GetComponent<ObjectData>().IsSelected)
                lastHitTransform.GetComponent<Outline>().OutlineMode = Outline.Mode.OutlineHidden;

            if (Input.GetButtonDown("Fire1") && !selectedObjects.Contains(currentTransform.GetComponent<ObjectData>()))
            {
                if (!multiSelectOn) DeselectAllObjects();

                SelectObject(currentTransform.GetComponent<ObjectData>());
            }
            else
                HoverObject(currentTransform.GetComponent<ObjectData>());

            lastHitTransform = hitInfo.transform;
        }
        else
        {
            if (selectedObjects.Count > 0 && Input.GetButtonDown("Fire1"))
            {
                DeselectAllObjects();

                lastHitTransform = null;
                return;
            }

            if (lastHitTransform != null)
            {
                DehoverObject(lastHitTransform.GetComponent<ObjectData>());
                return;
            }
        }


    }

    private void SelectObject(ObjectData obj)
    {
        Outline objOutline = obj.transform.GetComponent<Outline>();

        obj.IsSelected = true;
        selectedObjects.Add(obj);

        objOutline.OutlineColor = selectedColor;
        objOutline.OutlineMode = Outline.Mode.OutlineVisible;
    }

    private void DeselectAllObjects()
    {
        List<ObjectData> objectsToDeselect = new(selectedObjects);

        foreach (ObjectData _o in objectsToDeselect) DeselectObject(_o);

        if (pivot.isActiveAndEnabled)
        {
            RemoveTransformGizmos(pivot);
            pivot.gameObject.SetActive(false);
        }
    }

    private void HoverObject(ObjectData obj, Color customCol)
    {
        Outline objOutline = obj.transform.GetComponent<Outline>();

        objOutline.OutlineColor = customCol;
        objOutline.OutlineMode = Outline.Mode.OutlineVisible;
    }

    private void HoverObject(ObjectData obj)
    {
        Outline objOutline = obj.transform.GetComponent<Outline>();
        objOutline.OutlineMode = Outline.Mode.OutlineVisible;

        if (obj.IsSelected) objOutline.OutlineColor = mixedColor;
        else objOutline.OutlineColor = hoverColor;
    }

    private void DehoverObject(ObjectData obj)
    {
        if (!obj.IsSelected) obj.transform.GetComponent<Outline>().OutlineMode = Outline.Mode.OutlineHidden;
        else obj.transform.GetComponent<Outline>().OutlineColor = selectedColor;
    }
    private void DeselectObject(ObjectData obj)
    {
        if (obj == null) return;

        Outline objOutline = obj.transform.GetComponent<Outline>();

        obj.IsSelected = false;
        selectedObjects.Remove(obj);

        objOutline.OutlineColor = hoverColor;
        objOutline.OutlineMode = Outline.Mode.OutlineHidden;
    }

    private void SetTransformGizmos(ObjectData obj)
    {
        Dictionary<RuntimeTransformGizmo.TransformType, RuntimeTransformGizmo> connectedGizmos = obj.connectedGizmos;

        foreach (RuntimeTransformGizmo.TransformType type in Enum.GetValues(typeof(RuntimeTransformGizmo.TransformType)))
        {
            // temporary while I implement all of them
            if (type == RuntimeTransformGizmo.TransformType.Linear) continue;

            if (type == RuntimeTransformGizmo.TransformType.Rotation) continue;

            if (type == RuntimeTransformGizmo.TransformType.Scale) continue;

            if (type == RuntimeTransformGizmo.TransformType.X) continue;

            if (type == RuntimeTransformGizmo.TransformType.Y) continue;

            if (type == RuntimeTransformGizmo.TransformType.Z) continue;

            switch (EditorSelectionState)
            {
                case SelectionState.Linear:
                    if ((type & RuntimeTransformGizmo.TransformType.Linear) == 0) continue;
                    break;
                case SelectionState.Rotation:
                    if ((type & RuntimeTransformGizmo.TransformType.Rotation) == 0) continue;
                    break;
                case SelectionState.Scale:
                    if ((type & RuntimeTransformGizmo.TransformType.Scale) == 0) continue;
                    break;

                default:
                    break;
            }

            if (connectedGizmos.ContainsKey(type))
            {
                connectedGizmos[type].gameObject.SetActive(true);
                continue;
            }

            connectedGizmos[type] = RuntimeTransformGizmo.CreateGizmo(type, obj);

            connectedGizmos[type].gameObject.SetActive(true);
        }
    }

    private void RemoveTransformGizmos(ObjectData obj)
    {
        //Debug.Log($"Connected gizmo for {obj.name} is null: {obj.connectedGizmo == null}");
        foreach (RuntimeTransformGizmo.TransformType key in obj.connectedGizmos.Keys)
            obj.connectedGizmos[key].gameObject.SetActive(false);
    }

    private void AlterTransformGizmos()
    {
        if (!pivot.isActiveAndEnabled)
        {
            foreach (ObjectData obj in selectedObjects)
            {
                RemoveTransformGizmos(obj);
                SetTransformGizmos(obj);
            }
        }
        else
        {
            RemoveTransformGizmos(pivot);
            SetTransformGizmos(pivot);
        }
    }

    private void HandleGizmoSelecting()
    {
        if (selectedObjects.Count == 0)
        {
            lastHitGizmo = null;
            return;
        }

        bool isGizmoHit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hitInfo, 20000f, editorGizmoLayer);
        if (isGizmoHit)
        {
            RuntimeTransformGizmo rtg = hitInfo.transform.GetComponentInParent<RuntimeTransformGizmo>();

            if (lastHitGizmo != null)
            {
                if (lastHitGizmo.IsHeld && lastHitGizmo != rtg) return;

                else if (lastHitGizmo != rtg) DehoverGizmo(lastHitGizmo);
            }

            if (Input.GetButtonDown("Fire1"))
                SelectGizmo(rtg);
            else if (rtg.IsSelected && !rtg.IsHeld)
            {
                DeselectGizmo(rtg);
                HoverGizmo(rtg);
            }
            else
            {
                HoverGizmo(rtg);
            }

            lastHitGizmo = rtg;
        }
        else if (lastHitGizmo != null && !lastHitGizmo.IsHeld)
        {
            DeselectGizmo(lastHitGizmo);
            DehoverGizmo(lastHitGizmo);
            lastHitGizmo = null;
        }

    }

    private void SelectGizmo(RuntimeTransformGizmo gizmo)
    {
        //Debug.Log($"Gizmo Selected: {gizmo.gameObject.name}");
        gizmo.IsSelected = true;
    }

    private void DeselectGizmo(RuntimeTransformGizmo gizmo)
    {
        if (gizmo != null && !gizmo.Equals(null))
        {
            gizmo.IsSelected = false;
        }
    }

    private void HoverGizmo(RuntimeTransformGizmo gizmo)
    {
        //Debug.Log($"Gizmo being hovered: {gizmo.gameObject.name}");
        gizmo.SetAxisColor(gizmo.axisColor * 0.5f);
    }

    private void DehoverGizmo(RuntimeTransformGizmo gizmo)
    {
        //Debug.Log($"Gizmo being Dehovered: {gizmo.gameObject.name}");
        gizmo.SetAxisColor(RuntimeTransformGizmo.GetStandardAxisColor(gizmo));
    }

    private void SetSelectedGizmoColor(RuntimeTransformGizmo gizmo)
    {
        float brightness = 0.25f;
        Color newCol = Color.Lerp(RuntimeTransformGizmo.GetStandardAxisColor(gizmo), Color.white, Mathf.Clamp01(brightness));
        gizmo.SetAxisColor(newCol);
    }

    private Vector3 FindObjectsMidpoint(ObjectData[] objList)
    {
        Vector3 posVecSum = Vector3.zero;
        foreach (ObjectData obj in objList) posVecSum += obj.transform.position;
        return posVecSum / objList.Length;
    }

    public static Color GetHoverColor()
    {
        return new Color(hoverColor.r, hoverColor.g, hoverColor.b);
    }
}
