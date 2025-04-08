using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LevelData
{
    public int levelID;
    public List<ObjectInfo> objectInfos = new();
}

[System.Serializable]
public class ObjectInfo
{
    public int objID;
    public Vector3 position;
    public Quaternion rotation;
    public Vector3 scale;
}
