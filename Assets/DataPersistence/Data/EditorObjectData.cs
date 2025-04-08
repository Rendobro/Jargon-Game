using UnityEngine;

[System.Serializable]
public class ObjectData : MonoBehaviour
{
    public int objID;
    public Vector3 position;
    public Quaternion rotation;
    public Vector3 scale;
    
    private void Start()
    {
        ObjectData[] objects = FindObjectsByType<ObjectData>(FindObjectsSortMode.None);
    }
}
