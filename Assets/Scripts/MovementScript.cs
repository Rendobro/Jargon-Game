using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class MovementScript : MonoBehaviour
{
    public CharacterController ctrl;
    public Transform sphereLoc;
    public LayerMask groundMask;
    private Vector3 velocity;
    public float gravity = -9.81f;
    public float terminalVelocity = 50f;
    public float jumpPower = 5;
    public float speed = 5;
    private bool isGrounded;

    void Update()
    {
        isGrounded = Physics.CheckSphere(sphereLoc.position, 0.2f, groundMask);
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }
        MoveDetector();
        if (Input.GetButton("Jump") && isGrounded)
        {
            Jump();
        }
        velocity.y += gravity * Time.deltaTime;
        velocity.y = Mathf.Min(velocity.y,terminalVelocity);
        ctrl.Move(velocity * Time.deltaTime);
    }

    private void MoveDetector()
    {
        float vInput = Input.GetAxis("Vertical");  // W/S or Up/Down arrows
        float hInput = Input.GetAxis("Horizontal"); // A/D or Left/Right arrows

        Vector3 moveDirection = transform.forward * vInput + transform.right * hInput;

        ctrl.Move(moveDirection * speed * Time.deltaTime);
    }

    public void Jump()
    {
        velocity.y = Mathf.Sqrt(Mathf.Abs(2 * gravity * jumpPower));
    }
}

