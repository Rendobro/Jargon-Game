using UnityEngine;
using UnityEngine.InputSystem;

public class EditorControllerScript : MonoBehaviour, IDataPersistence
{
    [SerializeField] Camera cam;

    private float xRotation = 0f;
    private float editorSensitivity = 400f;
    private InputSystem_Actions inputActions;

    private void Start()
    {
        inputActions = new InputSystem_Actions();
    }

    private void OnEnable()
    {
        inputActions.Enable();
    }

    private void OnDisable()
    {
        inputActions.Disable();
    }

    private void Update()
    {
        RotationDetector();
    }

    private void RotationDetector()
    {
        Debug.Log("here!");
        if (inputActions.UI.MouseDrag.inProgress)
        {
            Vector2 mouseDelta = inputActions.UI.MouseDelta.ReadValue<Vector2>();
            Debug.Log($"Mouse Delta {mouseDelta}");
            float mouseX = mouseDelta.x * editorSensitivity * Time.deltaTime;
            float mouseY = mouseDelta.y * editorSensitivity * Time.deltaTime;

            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, -90f, 90f);

            cam.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

            transform.Rotate(Vector3.up * mouseX);
        }
    }

    public void LoadData(GameData data)
    {
        editorSensitivity = data.editorSensitivity;
    }

    public void SaveData(ref GameData data)
    {
        data.editorSensitivity = editorSensitivity;
    }
}
