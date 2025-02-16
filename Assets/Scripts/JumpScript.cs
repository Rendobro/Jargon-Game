using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpScript : MonoBehaviour
{
    public MovementScript moveCS;
    public CharacterController ctrl;
    
    // Start is called before the first frame update
    void Start()
    {
        moveCS = GameObject.FindGameObjectWithTag("Movement").GetComponent<MovementScript>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.E) && Input.GetKey(KeyCode.Y) && Input.GetKey(KeyCode.L) && Input.GetKey(KeyCode.Q))
        {
            moveCS.Jump();
        }
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.layer == 6)
        {
        Debug.Log("Hit!");
        }
    }
}
