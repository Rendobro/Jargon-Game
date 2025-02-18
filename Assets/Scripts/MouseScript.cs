using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseScript : MonoBehaviour
{
    public Transform player;
    public float sensitivity = 400f;
    private float xRot = 0f;
    // Start is called before the first frame update
    void Start()
    {
        PlayerPrefs.SetFloat("sensitivity",sensitivity);
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        float mouseX = Input.GetAxisRaw("Mouse X") * sensitivity * Time.deltaTime;
        float mouseY = Input.GetAxisRaw("Mouse Y") * sensitivity * Time.deltaTime;

        xRot -= mouseY;
        xRot = Mathf.Clamp(xRot,-90,90); //Clamp Vertical Rotation

        transform.localRotation = Quaternion.Euler(xRot, 0f, 0f);
        player.transform.Rotate(Vector3.up * mouseX);
    }
}