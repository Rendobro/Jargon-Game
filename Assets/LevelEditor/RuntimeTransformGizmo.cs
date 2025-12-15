using System;
using Unity.VisualScripting;
using UnityEngine;

public class RuntimeTransformGizmo : MonoBehaviour
{
    private Color axisColor = new();
    private Material axisMaterial;
    public ObjectData connectedObj;
    private bool isSelected;
    public bool IsSelected
    {
        get => isSelected;

        set
        {
            if (isSelected != value)
            {
                isSelected = value;
                if (EventManager.Instance != null && value == true)
                {
                    EventManager.Instance.OnGizmoSelected.Invoke(this);
                }
            }
        }
    }
    [SerializeField] private TransformType transformType;
    [SerializeField] private static EditorGizmoPrefabsContainer gizmoContainer;

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
        axisMaterial = new Material(gizmoContainer.gizmoBaseMaterial);
        SetMatColor(axisColor);
    }

    [ContextMenu("Create Gizmo")]
    public void CreateGizmo()
    {
        RuntimeTransformGizmo rtgTest = CreateGizmo(TransformType.LinearY,FindAnyObjectByType<ObjectData>());
        Debug.Log("rtg parent's name " + rtgTest.transform.parent.name);
    }

    private void SetMatColor(Color color)
    {
        axisColor = color;
        axisMaterial.SetColor("_AxisColor", color);
    }

    // Put this method in another class so RuntimeTransformGizmo can be just for obj instances and not for creation
    public static RuntimeTransformGizmo CreateGizmo(TransformType type, ObjectData connectedObj)
    {
        Transform connectedTransform = connectedObj.transform;
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
                gizmo = Instantiate(gizmoContainer.rtgPrefabs[0],connectedTransform);
                gizmo.transform.rotation = Quaternion.LookRotation(directionLinear,Vector3.up);
                Debug.Log($"Gizmo instantiated: {gizmo.name} \nGizmo's parent {gizmo.transform.parent.name}");
                break;
            case TransformType.Rotation:
                Vector3 circleNormal = axisDirs;

                //assign this to a prefab instance in the gizmo container
                gizmo = Instantiate(gizmoContainer.rtgPrefabs[1],connectedTransform);
                break;
            case TransformType.Scale:
                Vector3 directionScale = axisDirs;

                //assign this to a prefab instance in the gizmo container
                gizmo = Instantiate(gizmoContainer.rtgPrefabs[2],connectedTransform);
                break;
            default:
                throw new System.ArgumentException("Gizmo with no type detected. Not supposed to happen.");
        }

        gizmo.transform.position = connectedTransform.localPosition;
        gizmo.transform.rotation = connectedTransform.rotation;
        
        gizmo.transformType = type;
        gizmo.axisColor = color;
        gizmo.SetMatColor(color);
        gizmo.connectedObj = connectedObj;

        return gizmo;
    }
}
