using UnityEngine;
using System.Collections.Generic;
using Unity.Services.CloudSave;

[CreateAssetMenu(fileName = "EditorPrefabsContainer", menuName = "Scriptable Objects/EditorPrefabsContainer")]
public class EditorPrefabsContainer : ScriptableObject
{
    [SerializeField] public List<ObjectData> editorPrefabs = new();
}
