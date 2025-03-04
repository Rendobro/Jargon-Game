using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class MouseScript : MonoBehaviour
{
    [SerializeField] private Transform _tPlayer;
    [SerializeField] private readonly float defaultSensitivity = 150f;
    private float sensitivity;
    private float xRot = 0f;
    private bool isDisabled = false;

    void Start()
    {
        // Collect sensitivity from player's settings
        sensitivity = PlayerPrefs.GetFloat("sensitivity",defaultSensitivity);
        LockCursor();
    }

    void Update()
    {
        if (isDisabled) return;

        MouseDetector();
    }

    private void MouseDetector()
    {
        float mouseX = Input.GetAxisRaw("Mouse X") * sensitivity * Time.deltaTime;
        float mouseY = Input.GetAxisRaw("Mouse Y") * sensitivity * Time.deltaTime;

        xRot -= mouseY;
        xRot = Mathf.Clamp(xRot,-90,90); //Clamp Vertical Rotation

        transform.localRotation = Quaternion.Euler(xRot, 0f, 0f);
        _tPlayer.transform.Rotate(Vector3.up * mouseX);
    }

    public static void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
    }

    public static void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void DisableRotation()
    {
        isDisabled = true;
    }

    public void EnableRotation()
    {
        isDisabled = false;
    }
}