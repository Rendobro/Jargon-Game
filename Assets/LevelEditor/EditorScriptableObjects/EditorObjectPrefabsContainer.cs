using UnityEngine;
using System.Collections.Generic;
using Unity.Services.CloudSave;

[CreateAssetMenu(fileName = "EditorObjectPrefabsContainer", menuName = "Scriptable Objects/EditorObjectPrefabsContainer")]
public class EditorObjectPrefabsContainer : ScriptableObject
{
    private static EditorObjectPrefabsContainer _instance;

    public static EditorObjectPrefabsContainer Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = Resources.Load<EditorObjectPrefabsContainer>(
                    "EditorObjectPrefabsContainer"
                );

                if (_instance == null)
                {
                    Debug.LogError(
                        "EditorObjectPrefabsContainer not found in Resources!"
                    );
                }
            }

            return _instance;
        }
    }
    [SerializeField] public List<ObjectData> editorPrefabs = new();
}
