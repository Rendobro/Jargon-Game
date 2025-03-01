using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbJumpScript : MonoBehaviour
{
    public MovementScript moveCS;
    private bool ableToJump = false;

    void Update()
    {
        if (!moveCS.IsPlayerGrounded() && Input.GetButtonDown("Jump"))
        {
            ableToJump = true;
        }
        else if (moveCS.IsPlayerGrounded() && Input.GetKeyDown(KeyCode.Space))
        {
            ableToJump = true;
        }
        else if (moveCS.IsPlayerGrounded())
        {
            ableToJump = false;
        }
        DevSecret();
    }

    private void DevSecret()
    {
        if (Input.GetKey(KeyCode.E) && Input.GetKey(KeyCode.Y) && Input.GetKey(KeyCode.L) && Input.GetKey(KeyCode.Q))
        {
            moveCS.Jump();
        }
    }
    private void OnTriggerStay(Collider hit) 
    {
        if (hit.gameObject.layer == 6 && Input.GetButton("Jump") && ableToJump)
        {
            ableToJump = false;
            moveCS.Jump();
        }
    }
}
