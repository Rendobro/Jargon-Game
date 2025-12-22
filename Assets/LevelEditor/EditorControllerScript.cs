using UnityEngine;
using UnityEngine.InputSystem;

public class EditorControllerScript : MonoBehaviour, IDataPersistence, InputSystem_Actions.IEditorActions
{
    [SerializeField] Camera cam;
    private float velocity = 0f;
    private const float acceleration = 1.5f;
    private float xRotation = 0f;
    [SerializeField] private float editorSensitivity = 400f;
    [SerializeField] private float editorSpeed = 3f;
    private bool isRightClickHeld = false;

    //private InputSystem_Actions inputActions;

    // private void Awake()
    // {
    //     inputActions = new InputSystem_Actions();
    // }

    // private void OnEnable()
    // {
    //     inputActions.Editor.SetCallbacks(this);
    //     inputActions.Enable();
    // }

    // private void OnDisable()
    // {
    //     inputActions.Disable();
    // }

    // private void OnDestroy()
    // {
    //     inputActions.Dispose();
    // }

    private void Update()
    {
        RotationDetector();
        MoveDetector();
    }

    private void RotationDetector()
    {
        //Debug.Log($"here! {isRightClickHeld}");
        if (isRightClickHeld || Input.GetButton("Fire2"))
        {
            //Vector2 mouseDelta = inputActions.Editor.MouseDelta.ReadValue<Vector2>();
            //Debug.Log($"Mouse Delta {mouseDelta}      editorSens:{editorSensitivity}");
            float mouseX = Input.GetAxisRaw("Mouse X") * editorSensitivity * Time.deltaTime;
            float mouseY = Input.GetAxisRaw("Mouse Y") * editorSensitivity * Time.deltaTime;

            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, -90f, 90f);

            cam.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

            transform.Rotate(Vector3.up * mouseX);
        }
    }

    private void MoveDetector()
    {
        float vInput = Input.GetAxis("Vertical");  // W/S or Up/Down arrows
        float hInput = Input.GetAxis("Horizontal"); // A/D or Left/Right arrows
        float oInput = Input.GetAxis("Orthogonal"); // Q/E for elevation management

        Vector3 moveDirection = vInput * transform.forward + oInput * transform.up + hInput * transform.right;

        if (moveDirection.magnitude < 0.01f && velocity >= 0f) velocity = 0f;
        else velocity += acceleration * Time.deltaTime;

        if (moveDirection.magnitude > 1) moveDirection = moveDirection.normalized;

        transform.position = transform.position + (editorSpeed + velocity) * Time.deltaTime * moveDirection;
    }

    public void LoadData(GameData data)
    {
        editorSensitivity = data.editorSensitivity;
    }

    public void SaveData(ref GameData data)
    {
        data.editorSensitivity = editorSensitivity;
    }

    public void OnMouseDrag(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            isRightClickHeld = true; // Start rotating
        }
        else if (context.canceled)
        {
            isRightClickHeld = false; // Stop rotating
        }
    }

    public void OnMouseDelta(InputAction.CallbackContext context)
    {
        return;
    }
}
