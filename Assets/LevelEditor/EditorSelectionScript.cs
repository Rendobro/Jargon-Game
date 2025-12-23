using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;
using System;
public class EditorSelectionScript : MonoBehaviour
{
    private readonly int editorObjectLayer = 1 << 8;
    private readonly int editorGizmoLayer = 1 << 9;
    private Transform lastHitTransform = null;
    private RuntimeTransformGizmo lastHitGizmo = null;
    [SerializeField] private static Color selectedColor;
    [SerializeField] private static Color hoverColor;
    private static Color mixedColor;
    private bool multiSelectOn = false;
    private readonly HashSet<ObjectData> selectedObjects = new();

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
        selectedColor = Color.green;
        hoverColor = Color.cyan;
        float mixAmount = 0.35f;
        Vector3 colorCoords = Vector3.Lerp(new Vector3(hoverColor.r, hoverColor.g, hoverColor.b), new Vector3(selectedColor.r, selectedColor.g, selectedColor.b), mixAmount);
        mixedColor = new Color(colorCoords.x, colorCoords.y, colorCoords.z);
    }

    private void OnEnable()
    {
        //Debug.Log($"Event Manager is null? {EventManager.Instance == null}");
        EventManager.Instance.OnObjectSelected.AddListener(SetTransformGizmos);
        EventManager.Instance.OnGizmoSelected.AddListener(SetSelectedGizmoColor);
        EventManager.Instance.OnObjectDeselected.AddListener(RemoveTransformGizmo);
        EventManager.Instance.OnSelectionStateChanged.AddListener(AlterTransformGizmos);
    }

    private void OnDisable()
    {
        EventManager.Instance.OnObjectSelected.RemoveListener(SetTransformGizmos);
        EventManager.Instance.OnGizmoSelected.RemoveListener(SetSelectedGizmoColor);
        EventManager.Instance.OnObjectDeselected.RemoveListener(RemoveTransformGizmo);
        EventManager.Instance.OnSelectionStateChanged.RemoveListener(AlterTransformGizmos);
    }

    void Update()
    {
        multiSelectOn = Input.GetButton("MultiSelect");
        HandleGizmoSelecting();
        HandleObjectSelecting();
        if (Input.GetKeyDown(KeyCode.J)) EditorSelectionState = SelectionState.Linear;
        if (Input.GetKeyDown(KeyCode.K)) EditorSelectionState = SelectionState.Rotation;
        if (Input.GetKeyDown(KeyCode.L)) EditorSelectionState = SelectionState.Scale;

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
        else if (selectedObjects.Count > 0 && Input.GetButtonDown("Fire1"))
        {
            DeselectAllObjects();

            lastHitTransform = null;
        }
        else if (lastHitTransform != null)
            DehoverObject(lastHitTransform.GetComponent<ObjectData>());


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

            Debug.Log($"type: {(RuntimeTransformGizmo.TransformType)type}");

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

            Debug.Log("test: " + connectedGizmos[type]);

            connectedGizmos[type].gameObject.SetActive(true);
        }
    }

    private void RemoveTransformGizmo(ObjectData obj)
    {
        //Debug.Log($"Connected gizmo for {obj.name} is null: {obj.connectedGizmo == null}");
        foreach (RuntimeTransformGizmo.TransformType key in obj.connectedGizmos.Keys)
            obj.connectedGizmos[key].gameObject.SetActive(false);
    }

    private void AlterTransformGizmos()
    {
        foreach (ObjectData obj in selectedObjects)
        {
            RemoveTransformGizmo(obj);
            SetTransformGizmos(obj);
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

    public static Color GetHoverColor()
    {
        return new Color(hoverColor.r, hoverColor.g, hoverColor.b);
    }
}
