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
                if (EventManager.Instance != null && value == true)
                {
                    EventManager.Instance.OnObjectSelected.Invoke(this);
                }
            }
        }
    }
    public Vector3 position;
    public Quaternion rotation;
    public Vector3 scale;

    public ObjectData(ObjectData obj)
    {
        this.position = obj.position;
        this.scale = obj.scale;
        this.isSelected = obj.isSelected;
        this.rotation = obj.rotation;
        this.objID = obj.objID;
        this.IsSelected = obj.IsSelected;
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

    public ObjectData_JB(ObjectData_JB obj) : base(obj)
    {
        this.jargonType = obj.jargonType;
    }
}