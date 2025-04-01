using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbJumpScript : MonoBehaviour
{
    public static event Action OnJumpOrbActivated;
    [SerializeField] MovementScript msPlayer;
    private bool ableToJump = false;

    void Update()
    {
        if (!msPlayer.IsPlayerGrounded() && Input.GetButtonDown("Jump"))
        {
            ableToJump = true;
        }
        else if (msPlayer.IsPlayerGrounded() && Input.GetKeyDown(KeyCode.Space))
        {
            ableToJump = true;
        }
        else if (msPlayer.IsPlayerGrounded())
        {
            ableToJump = false;
        }
        DevSecret();
    }

    private void DevSecret()
    {
        if (Input.GetKey(KeyCode.E) && Input.GetKey(KeyCode.Y) && Input.GetKey(KeyCode.L) && Input.GetKey(KeyCode.Q))
        {
            OnJumpOrbActivated?.Invoke();
        }
    }
    private void OnTriggerStay(Collider hit) 
    {
        if (hit.gameObject.layer == 6 && Input.GetButton("Jump") && ableToJump)
        {
            ableToJump = false;
            OnJumpOrbActivated?.Invoke();
        }
    }
}
