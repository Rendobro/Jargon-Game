using UnityEngine;
using System.Collections.Generic;
using Unity.Services.CloudSave;

[CreateAssetMenu(fileName = "EditorPrefabsContainer", menuName = "Scriptable Objects/EditorPrefabsContainer")]
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
                    "EditorPrefabsContainer"
                );

                if (_instance == null)
                {
                    Debug.LogError(
                        "EditorPrefabsContainer not found in Resources!"
                    );
                }
            }

            return _instance;
        }
    }
    [SerializeField] public List<ObjectData> editorPrefabs = new();
}
