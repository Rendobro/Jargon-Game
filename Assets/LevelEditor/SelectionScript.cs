using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
public class SelectionScript : MonoBehaviour
{
    private readonly int editorObjectLayer = 1 << 8;
    private Ray selector;
    private Transform lastHitTransform = null;
    private Color selectedColor;
    private Color hoverColor;

    private bool multiSelectOn = false;

    private HashSet<Transform> selectedObjects = new();

    private void Awake()
    {
        selectedColor = Color.green;
        hoverColor = Color.cyan;
    }

    void Update()
    {
        multiSelectOn = Input.GetButton("MultiSelect");
        HandleSelecting();
    }

    private void HandleSelecting()
    {
        bool isObjectHit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hitInfo, 20000f, editorObjectLayer);
        if (isObjectHit)
        {
            Transform currentTransform = hitInfo.transform;

            if (lastHitTransform != null && !currentTransform.Equals(lastHitTransform) && !lastHitTransform.GetComponent<ObjectData>().isSelected)
                lastHitTransform.GetComponent<Outline>().OutlineMode = Outline.Mode.OutlineHidden;

            if (Input.GetButtonDown("Fire1") && !selectedObjects.Contains(currentTransform))
            {
                if (!multiSelectOn) DeselectAllObjects();

                SelectObject(currentTransform);
            }
            else if (!currentTransform.GetComponent<ObjectData>().isSelected) 
                HoverObject(currentTransform);

            lastHitTransform = hitInfo.transform;
        }
        else if (selectedObjects.Count > 0 && Input.GetButtonDown("Fire1"))
            DeselectAllObjects();
        else if (lastHitTransform != null && !lastHitTransform.GetComponent<ObjectData>().isSelected)
            DehoverObject(lastHitTransform);
    }

    private void SelectObject(Transform obj)
    {
        Outline objOutline = obj.GetComponent<Outline>();
        selectedObjects.Add(obj);
        obj.GetComponent<ObjectData>().isSelected = true;
        objOutline.OutlineColor = selectedColor;
        objOutline.OutlineMode = Outline.Mode.OutlineVisible;
    }

    private void DeselectAllObjects()
    {
        List<Transform> objectsToDeselect = new List<Transform>(selectedObjects);

        foreach (Transform _t in objectsToDeselect)
        {
            DeselectObject(_t);
        }
    }

    private void HoverObject(Transform obj)
    {
        Outline objOutline = obj.GetComponent<Outline>();

        objOutline.OutlineColor = hoverColor;
        objOutline.OutlineMode = Outline.Mode.OutlineVisible;
    }

    private void DehoverObject(Transform obj)
    {
        obj.GetComponent<Outline>().OutlineMode = Outline.Mode.OutlineHidden;
    }

    private void DeselectObject(Transform obj)
    {
        Outline objOutline = obj.GetComponent<Outline>();
        selectedObjects.Remove(obj);
        obj.GetComponent<ObjectData>().isSelected = false;
        objOutline.OutlineColor = hoverColor;
        objOutline.OutlineMode = Outline.Mode.OutlineHidden;
    }
}
