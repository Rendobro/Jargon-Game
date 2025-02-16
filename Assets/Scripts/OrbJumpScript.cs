using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbJumpScript : MonoBehaviour
{
    private MovementScript moveCS;
    private bool ableToJump = false;
    
    // Start is called before the first frame update
    void Start()
    {
        moveCS = GameObject.FindGameObjectWithTag("Movement").GetComponent<MovementScript>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!moveCS.IsPlayerGrounded() && Input.GetButtonDown("Jump"))
        {
            ableToJump = true;
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
