using UnityEngine;
using System.Collections.Generic;
using Unity.Services.CloudSave;

[CreateAssetMenu(fileName = "EditorPrefabsContainer", menuName = "Scriptable Objects/EditorPrefabsContainer")]
public class EditorPrefabsContainer : ScriptableObject
{
    private static EditorPrefabsContainer _instance;

    public static EditorPrefabsContainer Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = Resources.Load<EditorPrefabsContainer>(
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
