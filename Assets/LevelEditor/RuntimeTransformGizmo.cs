using System;
using UnityEngine;

public class RuntimeTransformGizmo : MonoBehaviour
{
    private Color axisColor = new();
    private Material axisMaterial;
    [SerializeField] private TransformType transformType;
    [SerializeField] private readonly EditorGizmoPrefabsContainer gizmoContainer;

    public enum TransformType
    {
        Linear = 1 << 0,
        Rotation = 1 << 1,
        Scale = 1 << 2,
        X = 1 << 3,
        Y = 1 << 4,
        Z = 1 << 5,
        LinearX = Linear & X,
        LinearY = Linear & Y,
        LinearZ = Linear & Z,
        RotationX = Rotation & X,
        RotationY = Rotation & Y,
        RotationZ = Rotation & Z,
        ScaleX = Scale & X,
        ScaleY = Scale & Y,
        ScaleZ = Scale & Z,
    }

    private void Start()
    {
        axisMaterial = new Material(gizmoContainer.gizmoBaseMaterial);
        SetMatColor(axisColor);
    }

    private void SetMatColor(Color color)
    {
        axisColor = color;
        axisMaterial.SetColor("_AxisColor", color);
    }

    public static RuntimeTransformGizmo CreateGizmo(TransformType type, Transform connectedTransform)
    {
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
                Debug.LogError("Gizmo with no axis detected. Not supposed to happen.");
                throw new System.ArgumentException();
        }

        Vector3 axisDirs = Vector3.right * color.r + Vector3.up * color.g + Vector3.forward * color.b;
        GameObject newGizmoObject;

        switch (axisTypeFlags)
        {
            case TransformType.Linear:
                Vector3 directionLinear = axisDirs;

                //assign this to a prefab instance in the gizmo container
                newGizmoObject = new GameObject("LinearGizmo");
                break;
            case TransformType.Rotation:
                Vector3 circleNormal = axisDirs;

                //assign this to a prefab instance in the gizmo container
                newGizmoObject = new GameObject("RotationGizmo");
                newGizmoObject.transform.rotation = Quaternion.LookRotation(axisDirs,Vector3.up);
                break;
            case TransformType.Scale:
                Vector3 directionScale = axisDirs;

                //assign this to a prefab instance in the gizmo container
                newGizmoObject = new GameObject("ScaleGizmo");
                break;
            default:
                Debug.LogError("Gizmo with no type detected. Not supposed to happen.");
                throw new System.ArgumentException();
        }

        newGizmoObject.transform.position = connectedTransform.localPosition;
        newGizmoObject.transform.rotation = connectedTransform.rotation;
        newGizmoObject.transform.parent = connectedTransform;

        RuntimeTransformGizmo gizmo = newGizmoObject.AddComponent<RuntimeTransformGizmo>();

        gizmo.transformType = type;
        gizmo.axisColor = color;
        gizmo.SetMatColor(color);

        return gizmo;
    }
}
