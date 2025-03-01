using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Unity.Netcode;
public class MovementScript : NetworkBehaviour
{
    [SerializeField] private CharacterController ctrl;
    [SerializeField] private Transform sphereLoc;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private readonly float terminalVelocity = 50f;
    [SerializeField] private float jumpPower = 5;
    [SerializeField] private float speed = 5;
    [SerializeField] private readonly float checkSphereRadius = 0.5f;
    [SerializeField] private readonly short defaultGravity = 30;
    public float gravity;
    private Vector3 velocity;
    private bool isGrounded;
    private bool isDisabled = false;

    void Start()
    {
        //if (!IsOwner) Destroy(this);
        gravity = -PlayerPrefs.GetFloat("gravity",defaultGravity);
    }
    void FixedUpdate()
    {
        if (isDisabled) return;
        isGrounded = Physics.CheckSphere(sphereLoc.position, checkSphereRadius, groundMask);
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

        if (moveDirection.magnitude > 1) moveDirection = moveDirection.normalized;

        ctrl.Move(speed * Time.deltaTime * moveDirection);
    }

    public void Jump()
    {
        velocity.y = Mathf.Sqrt(Mathf.Abs(2 * gravity * jumpPower));
    }

    public void ChangeVelocityVertical(float speed)
    {
        velocity.y = speed;
    }

    public void ResetVelocityVertical()
    {
        velocity.y = 0f;
    }

    public void DisableMovement()
    {
        isDisabled = true;
    }
    public void EnableMovement()
    {
        isDisabled = false;
    }
    public bool IsPlayerGrounded()
    {
        return isGrounded;
    }

    public void ChangeSpeed(float newSpeed)
    {
        speed = newSpeed;
    }
    public void ChangeJumpPower(float newJumpPower)
    {
        jumpPower = newJumpPower;
    }
}

