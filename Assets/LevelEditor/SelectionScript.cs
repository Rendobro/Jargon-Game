using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;
public class SelectionScript : MonoBehaviour
{
    private readonly int editorObjectLayer = 1 << 8;
    private Transform lastHitTransform = null;
    private static Color selectedColor;
    private static Color hoverColor;
    private static Color mixedColor;
    private bool multiSelectOn = false;

    private readonly HashSet<ObjectData> selectedObjects = new();

    private void Awake()
    {
        selectedColor = Color.green;
        hoverColor = Color.cyan;
        float mixAmount = 0.35f;
        Vector3 colorCoords = Vector3.Lerp(new Vector3(hoverColor.r,hoverColor.g,hoverColor.b),new Vector3(selectedColor.r,selectedColor.g,selectedColor.b), mixAmount);
        mixedColor = new Color(colorCoords.x,colorCoords.y,colorCoords.z);
    }

    private void OnEnable()
    {
        Debug.Log($"Event Manager is null? {EventManager.Instance == null}");
        EventManager.Instance.OnObjectSelected.AddListener(PrintSelected);
    }

    private void OnDisable()
    {
        EventManager.Instance.OnObjectSelected.RemoveListener(PrintSelected);
    }

    void Update()
    {
        multiSelectOn = Input.GetButton("MultiSelect");
        HandleSelecting();
    }

    private void PrintSelected(ObjectData obj)
    {
        Debug.Log(obj.name + " has been selected!");
    }

    private void HandleSelecting()
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
            else if (!currentTransform.GetComponent<ObjectData>().IsSelected)
                HoverObject(currentTransform.GetComponent<ObjectData>(), hoverColor);
            else
                HoverObject(currentTransform.GetComponent<ObjectData>(), mixedColor);

            lastHitTransform = hitInfo.transform;
        }
        else if (selectedObjects.Count > 0 && Input.GetButtonDown("Fire1"))
        {
            DeselectAllObjects();

            lastHitTransform = null;
        }
        else if (lastHitTransform != null && !lastHitTransform.GetComponent<ObjectData>().IsSelected)
            DehoverObject(lastHitTransform.GetComponent<ObjectData>());
        else if (lastHitTransform != null)
            lastHitTransform.GetComponent<Outline>().OutlineColor = selectedColor;

    }

    private void SelectObject(ObjectData obj)
    {
        Outline objOutline = obj.transform.GetComponent<Outline>();
        selectedObjects.Add(obj);
        obj.IsSelected = true;
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

    private void DehoverObject(ObjectData obj) => obj.transform.GetComponent<Outline>().OutlineMode = Outline.Mode.OutlineHidden;
    private void DeselectObject(ObjectData obj)
    {
        Outline objOutline = obj.transform.GetComponent<Outline>();
        selectedObjects.Remove(obj);
        obj.IsSelected = false;
        objOutline.OutlineColor = hoverColor;
        objOutline.OutlineMode = Outline.Mode.OutlineHidden;
    }

    public static Color GetHoverColor()
    {
        return new Color(hoverColor.r, hoverColor.g, hoverColor.b);
    }
}
