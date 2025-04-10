using UnityEngine;

public class SelectionScript : MonoBehaviour
{
    private int editorObjectLayer = 1 << 8;
    private Ray selector;
    private Transform lastHitTransform = null;

    void Update()
    {
        selector = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(selector, out RaycastHit hitInfo, 200000f, editorObjectLayer))
        {
            if (lastHitTransform != null && !hitInfo.transform.Equals(lastHitTransform)) lastHitTransform.GetComponent<Outline>().OutlineMode = Outline.Mode.OutlineHidden;
            
            lastHitTransform = hitInfo.transform;
            lastHitTransform.GetComponent<Outline>().OutlineMode = Outline.Mode.OutlineVisible;
        }
        else if (lastHitTransform != null)
        {
            lastHitTransform.GetComponent<Outline>().OutlineMode = Outline.Mode.OutlineHidden;
        }
    }
}
