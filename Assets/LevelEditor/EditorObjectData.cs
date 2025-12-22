using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public class ObjectData : MonoBehaviour
{
    public int objID;
    private bool isSelected;
    public bool IsSelected
    {
        get => isSelected;

        set
        {
            if (isSelected != value)
            {
                isSelected = value;
                if (EventManager.Instance == null) return;
                if (value == true)
                    EventManager.Instance.OnObjectSelected.Invoke(this);
                else
                    EventManager.Instance.OnObjectDeselected.Invoke(this);

            }
        }
    }
    public GameObject gizmosParent;
    public Dictionary<RuntimeTransformGizmo.TransformType, RuntimeTransformGizmo> connectedGizmos = new();
    public Vector3 position;
    public Quaternion rotation;
    public Vector3 scale;
    private void Awake()
    {
        gizmosParent = new GameObject("GizmoParent");
        gizmosParent.transform.parent = this.transform;
        gizmosParent.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
    }
}

[System.Serializable]
public class ObjectData_JB : ObjectData
{
    public enum JargonType
    {
        Respawn = 1 << 0,
        MovingPlatform = 1 << 1,
        Timer = 1 << 2,
    }

    public JargonType jargonType;
}