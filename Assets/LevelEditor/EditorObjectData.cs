using UnityEngine;

[System.Serializable]
public class ObjectData : MonoBehaviour
{
    public int objID;
    public Vector3 position;
    public Quaternion rotation;
    public Vector3 scale;
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
}