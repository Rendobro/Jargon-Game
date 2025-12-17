using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "EditorGizmoPrefabsContainer", menuName = "Scriptable Objects/EditorGizmoPrefabsContainer")]
[System.Serializable]
public class EditorGizmoPrefabsContainer : ScriptableObject
{
    private static EditorGizmoPrefabsContainer _instance;

    public static EditorGizmoPrefabsContainer Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = Resources.Load<EditorGizmoPrefabsContainer>(
                    "EditorGizmoPrefabsContainer"
                );

                if (_instance == null)
                {
                    Debug.LogError(
                        "EditorGizmoPrefabsContainer not found in Resources!"
                    );
                }
            }

            return _instance;
        }
    }

    [SerializeField] public Material gizmoBaseMaterial;
    [SerializeField] public List<RuntimeTransformGizmo> rtgPrefabs = new();
}
