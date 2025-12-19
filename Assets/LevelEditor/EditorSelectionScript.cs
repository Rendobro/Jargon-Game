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
        EventManager.Instance.OnObjectSelected.AddListener(SetTransformGizmo);
        EventManager.Instance.OnGizmoSelected.AddListener(SetSelectedGizmoColor);
        EventManager.Instance.OnObjectDeselected.AddListener(RemoveTransformGizmo);
    }

    private void OnDisable()
    {
        EventManager.Instance.OnObjectSelected.RemoveListener(SetTransformGizmo);
        EventManager.Instance.OnGizmoSelected.RemoveListener(SetSelectedGizmoColor);
        EventManager.Instance.OnObjectSelected.RemoveListener(RemoveTransformGizmo);
    }

    void Update()
    {
        multiSelectOn = Input.GetButton("MultiSelect");
        HandleObjectSelecting();
        HandleGizmoSelecting();
    }

    private void SetTransformGizmo(ObjectData obj)
    {

        if (obj.connectedGizmo == null)
        {
            obj.connectedGizmo = RuntimeTransformGizmo.
            CreateGizmo(RuntimeTransformGizmo.TransformType.LinearZ, obj);
        }
        obj.connectedGizmo.gameObject.SetActive(true);
        //Debug.Log(obj.name + " has been selected!");
    }

    private void RemoveTransformGizmo(ObjectData obj)
    {
        //Debug.Log($"Connected gizmo for {obj.name} is null: {obj.connectedGizmo == null}");
        obj.connectedGizmo.gameObject.SetActive(false);
    }

    private void HandleObjectSelecting()
    {
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
        else if (lastHitGizmo != null && Input.GetButton("Fire1"))
        {
            DehoverGizmo(lastHitGizmo);
        }
        else if (lastHitGizmo != null)
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
