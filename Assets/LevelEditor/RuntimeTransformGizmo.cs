using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class RuntimeTransformGizmo : MonoBehaviour
{
    [HideInInspector] public Color axisColor = Color.darkOliveGreen;
    private Material axisMaterial;
    private Renderer[] childRenderers;
    public ObjectData Target { get; private set; }

    public TransformType transformType;

    private bool isSelected;
    public bool IsHeld => isSelected && Input.GetButton("Fire1");

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


    private void Awake()
    {
        childRenderers = GetComponentsInChildren<Renderer>(includeInactive: true);
    }
    private void Start()
    {
        axisMaterial = new Material(EditorGizmoPrefabsContainer.Instance.gizmoBaseMaterial);
        foreach (Renderer r in childRenderers)
            r.material = axisMaterial; // instance per rendereR

        SetAxisColor(axisColor);
    }

    public void SetAxisColor(Color color)
    {
        axisColor = color;
        if (axisMaterial == null) return;
        foreach (var r in childRenderers)
            r.material.SetColor("_AxisColor", color);
    }
    public static RuntimeTransformGizmo CreateGizmo(TransformType type, ObjectData connectedObj)
    {
        if (activeObjects.Contains(connectedObj)) return null;
        Transform connectedTransform = connectedObj.transform;
        Quaternion coolRot;

        activeObjects.Add(connectedObj);
        TransformType axisDirectionFlags = type & (TransformType.X | TransformType.Y | TransformType.Z);
        TransformType axisTypeFlags = type & (TransformType.Linear | TransformType.Rotation | TransformType.Scale);

        Color color = new Color(0, 0, 0);

        switch (axisDirectionFlags)
        {
            case TransformType.X:
                coolRot = Quaternion.Euler(0f, 0f, 0f);  // X along local right

                color = Color.red;
                break;
            case TransformType.Y:
                coolRot = Quaternion.Euler(0f, 0f, 90f);  // X along local right

                color = Color.green;
                break;
            case TransformType.Z:
                coolRot = Quaternion.Euler(0f, -90f, 0f);  // X along local right

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
                gizmo.transform.localRotation = coolRot;
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
        gizmo.Target = connectedObj;
        gizmo.transformType = type;
        gizmo.SetAxisColor(color);
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
