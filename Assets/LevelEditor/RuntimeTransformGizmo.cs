using System;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using Newtonsoft.Json.Converters;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class RuntimeTransformGizmo : MonoBehaviour
{
    [HideInInspector] public Color axisColor = Color.darkOliveGreen;
    private Material axisMaterial;
    private Renderer[] childRenderers;
    public ObjectData Target { get; private set; }
    private readonly float apparentSize = 0.3f;
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
                    EventManager.Instance.OnGizmoSelected.Invoke(this);
                else
                    EventManager.Instance.OnGizmoDeselected.Invoke(this);
            }
        }
    }
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

    private void LateUpdate()
    {
        Camera cam = Camera.main;

        // Weird matrix math that magically makes gizmo
        // scale look normal with non-uniform object scalings
        Matrix4x4 invRS = Matrix4x4.Rotate(Target.transform.rotation).inverse *
                          Matrix4x4.Scale(Target.transform.lossyScale).inverse;

        Vector3 signVec = new Vector3(Math.Sign(invRS.lossyScale.x) * Math.Sign(Target.transform.lossyScale.x),Math.Sign(Target.transform.lossyScale.y),Math.Sign(Target.transform.lossyScale.z));
        Target.gizmosParent.transform.localScale = Vector3.Scale(invRS.lossyScale, signVec);

        // Screen perspective scaling
        float distance = Vector3.Distance(cam.transform.position, transform.position);
        transform.localScale = Vector3.one * distance * apparentSize;

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
        if (connectedObj.connectedGizmos.ContainsKey(type)) return connectedObj.connectedGizmos[type];
        Quaternion coolRot;
        Transform gizmoParentTransform = connectedObj.gizmosParent.transform;
        TransformType axisDirectionFlags = type & (TransformType.X | TransformType.Y | TransformType.Z);
        TransformType axisTypeFlags = type & (TransformType.Linear | TransformType.Rotation | TransformType.Scale);

        Color color = new Color(0, 0, 0);

        switch (axisDirectionFlags)
        {
            case TransformType.X:
                coolRot = Quaternion.Euler(0f, 0f, 90f);
                color = Color.red;
                break;
            case TransformType.Y:
                coolRot = Quaternion.Euler(0f, 0f, 0f);
                color = Color.green;
                break;
            case TransformType.Z:
                coolRot = Quaternion.Euler(-90f, 0f, 0f);
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
                gizmo = Instantiate(EditorGizmoPrefabsContainer.Instance.rtgPrefabs[0], gizmoParentTransform);
                gizmo.transform.Rotate(coolRot.eulerAngles, Space.Self);
                Debug.Log($"Gizmo instantiated: {gizmo.name} \nGizmo's parent {gizmo.transform.parent.name}");
                break;
            case TransformType.Rotation:
                Vector3 circleNormal = axisDirs;
                gizmo = Instantiate(EditorGizmoPrefabsContainer.Instance.rtgPrefabs[1], gizmoParentTransform);
                gizmo.transform.Rotate(coolRot.eulerAngles, Space.Self);
                break;
            case TransformType.Scale:
                Vector3 directionScale = axisDirs;
                gizmo = Instantiate(EditorGizmoPrefabsContainer.Instance.rtgPrefabs[2], gizmoParentTransform);
                gizmo.transform.Rotate(coolRot.eulerAngles, Space.Self);
                break;
            default:
                throw new System.ArgumentException("Gizmo with no type detected. Not supposed to happen.");
        }
        gizmo.transform.localPosition = Vector3.zero;
        gizmo.Target = connectedObj;
        gizmo.transformType = type;
        gizmo.SetAxisColor(color);
        connectedObj.connectedGizmos[type] = gizmo;
        Debug.Log($"gizmo made: {gizmo}");
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

    public override string ToString()
    {
        return "TransformType: " + transformType.HumanName() +
        "\nAxisMaterial: " + axisMaterial +
        "\nTarget Object: " + Target;
    }
}
