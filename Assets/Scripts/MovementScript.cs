using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using psm = PlayerStatsManager;
using lfs = LevelFinishScript;
using ojs = OrbJumpScript;
using prm = PlayerResetManager;
using pms = PauseMenuScript;
public class MovementScript : MonoBehaviour
{
    [SerializeField] private CharacterController ctrl;
    [SerializeField] private Transform sphereLoc;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private readonly float terminalVelocity = 50f;
    [SerializeField] private float jumpPower = 5f;
    [SerializeField] private float speed = 3f;
    [SerializeField] private readonly float checkSphereRadius = 0.5f;
    private static float gravity;
    private Vector3 velocity;
    private bool isGrounded;
    private bool isDisabled = false;

    private void OnEnable()
    {     
        gravity = psm.Instance.GetGravity();
        EventManager.Instance.OnLevelFinish.AddListener(DisableMovement);
        EventManager.Instance.OnPlayerReset.AddListener(ResetVelocityVertical);
        EventManager.Instance.OnJumpOrbActivated.AddListener(Jump);
        EventManager.Instance.OnMenuPaused.AddListener(DisableMovement);
        EventManager.Instance.OnMenuUnpaused.AddListener(EnableMovement);
    }

    private void OnDisable()
    {
        EventManager.Instance.OnLevelFinish.RemoveListener(DisableMovement);
        EventManager.Instance.OnPlayerReset.RemoveListener(ResetVelocityVertical);
        EventManager.Instance.OnJumpOrbActivated.RemoveListener(Jump);
        EventManager.Instance.OnMenuPaused.RemoveListener(DisableMovement);
        EventManager.Instance.OnMenuUnpaused.RemoveListener(EnableMovement);
    }
    void FixedUpdate()
    {
        if (isDisabled) return;
        GroundCheck();
        if (isGrounded && velocity.y < 0)
        {
            
            velocity.y = -2f;
        }

        MoveDetector();
        JumpDetector();
        HandleVerticalVelocity();
    }

    private void MoveDetector()
    {
        float vInput = Input.GetAxis("Vertical");  // W/S or Up/Down arrows
        float hInput = Input.GetAxis("Horizontal"); // A/D or Left/Right arrows

        Vector3 moveDirection = transform.forward * vInput + transform.right * hInput;

        if (moveDirection.magnitude > 1) moveDirection = moveDirection.normalized;

        ctrl.Move(speed * Time.deltaTime * moveDirection);
    }

    private void JumpDetector() { if (Input.GetButton("Jump") && isGrounded) Jump(); }

    private void GroundCheck() => isGrounded = Physics.CheckSphere(sphereLoc.position, checkSphereRadius, groundMask);

    private void HandleVerticalVelocity()
    {
        velocity.y += gravity * Time.deltaTime;
        velocity.y = Mathf.Min(velocity.y,terminalVelocity);
        ctrl.Move(velocity * Time.deltaTime);
    }
    private void Jump() => velocity.y = Mathf.Sqrt(Mathf.Abs(2 * gravity * jumpPower));

    public void ChangeVelocityVertical(float speed) => velocity.y = speed;

    private void ResetVelocityVertical() => velocity.y = 0f;

    private void DisableMovement(int _) => isDisabled = true;

    private void DisableMovement() => isDisabled = true;

    private void EnableMovement() => isDisabled = false;

    public bool IsPlayerGrounded() => isGrounded;

    public void ChangeSpeed(float newSpeed) => speed = newSpeed;

    public void ChangeJumpPower(float newJumpPower) => jumpPower = newJumpPower;

    public static void SetGravity(float gravNew) => gravity = gravNew;
}

