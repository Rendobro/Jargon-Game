using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class RuntimeTransformGizmo : MonoBehaviour
{
    [HideInInspector] public Color axisColor = new();
    private Material axisMaterial;
    public TransformType transformType;

    private bool isSelected;
    public bool IsSelected
    {
        get => isSelected;

        set
        {
            if (isSelected != value)
            {
                isSelected = value;
                if (EventManager.Instance == null) return;
                if (value == true)
                {
                    EventManager.Instance.OnGizmoSelected.Invoke(this);
                }
                else
                {
                    EventManager.Instance.OnGizmoDeselected.Invoke(this);
                }
            }
        }
    }

    private static readonly HashSet<ObjectData> activeObjects = new();

    public enum TransformType
    {
        Linear = 1 << 0,
        Rotation = 1 << 1,
        Scale = 1 << 2,
        X = 1 << 3,
        Y = 1 << 4,
        Z = 1 << 5,
        LinearX = Linear | X,
        LinearY = Linear | Y,
        LinearZ = Linear | Z,
        RotationX = Rotation | X,
        RotationY = Rotation | Y,
        RotationZ = Rotation | Z,
        ScaleX = Scale | X,
        ScaleY = Scale | Y,
        ScaleZ = Scale | Z,
    }

    private void Start()
    {
        axisMaterial = new Material(EditorGizmoPrefabsContainer.Instance.gizmoBaseMaterial);
        SetColor(axisColor);
    }

    [ContextMenu("Create Gizmo")]
    public void CreateGizmo()
    {
        RuntimeTransformGizmo rtgTest = CreateGizmo(TransformType.LinearY, FindAnyObjectByType<ObjectData>());
        Debug.Log("rtg parent's name " + rtgTest.transform.parent.name);
    }

    public void SetColor(Color color)
    {
        axisColor = color;
        if (axisMaterial != null) axisMaterial.SetColor("_AxisColor", color);
    }
    public static RuntimeTransformGizmo CreateGizmo(TransformType type, ObjectData connectedObj)
    {
        if (activeObjects.Contains(connectedObj)) return null;
        Transform connectedTransform = connectedObj.transform;
        activeObjects.Add(connectedObj);
        TransformType axisDirectionFlags = type & (TransformType.X | TransformType.Y | TransformType.Z);
        TransformType axisTypeFlags = type & (TransformType.Linear | TransformType.Rotation | TransformType.Scale);

        Color color;

        switch (axisDirectionFlags)
        {
            case TransformType.X:
                color = Color.red;
                break;
            case TransformType.Y:
                color = Color.green;
                break;
            case TransformType.Z:
                color = Color.blue;
                break;
            default:
                throw new System.ArgumentException("Gizmo with no axis detected. Not supposed to happen.");
        }

        Vector3 axisDirs = Vector3.right * color.r + Vector3.up * color.g + Vector3.forward * color.b;
        RuntimeTransformGizmo gizmo;

        switch (axisTypeFlags)
        {
            case TransformType.Linear:
                Vector3 directionLinear = axisDirs;

                //assign this to a prefab instance in the gizmo container
                gizmo = Instantiate(EditorGizmoPrefabsContainer.Instance.rtgPrefabs[0], connectedTransform);
                gizmo.transform.rotation = Quaternion.LookRotation(directionLinear, Vector3.up);
                Debug.Log($"Gizmo instantiated: {gizmo.name} \nGizmo's parent {gizmo.transform.parent.name}");
                break;
            case TransformType.Rotation:
                Vector3 circleNormal = axisDirs;

                //assign this to a prefab instance in the gizmo container
                gizmo = Instantiate(EditorGizmoPrefabsContainer.Instance.rtgPrefabs[1], connectedTransform);
                break;
            case TransformType.Scale:
                Vector3 directionScale = axisDirs;

                //assign this to a prefab instance in the gizmo container
                gizmo = Instantiate(EditorGizmoPrefabsContainer.Instance.rtgPrefabs[2], connectedTransform);
                break;
            default:
                throw new System.ArgumentException("Gizmo with no type detected. Not supposed to happen.");
        }

        gizmo.transform.position = connectedTransform.localPosition;
        gizmo.transform.rotation = connectedTransform.rotation;

        gizmo.transformType = type;
        gizmo.SetColor(color);
        return gizmo;
    }
    public static Color GetStandardAxisColor(RuntimeTransformGizmo gizmo)
    {
        if (gizmo == null)
        { 
            Debug.LogError("Cant check gizmo color because it is null");
            return Color.rosyBrown;
        }

        TransformType gType = gizmo.transformType;
        if ((gType & TransformType.X) != 0)
        {
            return Color.red;
        }
        if ((gType & TransformType.Y) != 0)
        {
            return Color.green;
        }
        if ((gType & TransformType.Z) != 0)
        {
            return Color.blue;
        }

        //indicate a problem
        return Color.hotPink;
    }
}
