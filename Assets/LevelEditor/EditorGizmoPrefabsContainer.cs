using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "EditorGizmoPrefabsContainer", menuName = "Scriptable Objects/EditorGizmoPrefabsContainer")]
public class EditorGizmoPrefabsContainer : ScriptableObject
{
    [SerializeField] public Material gizmoBaseMaterial;
    [SerializeField] public List<RuntimeTransformGizmo> rtgPrefabs = new();
}
