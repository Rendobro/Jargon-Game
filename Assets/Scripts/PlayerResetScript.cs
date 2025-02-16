using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerResetScript : MonoBehaviour
{
    private MovementScript moveCS;
    public CharacterController ctrl;
    public Rigidbody rb;
    public Camera cam;
    // Start is called before the first frame update
    void Start()
    {
        moveCS = GameObject.FindGameObjectWithTag("Movement").GetComponent<MovementScript>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void ResetChar()
    {
        Debug.Log(transform.position);
        ctrl.enabled = false;
        transform.SetPositionAndRotation(new Vector3(0,5,0), Quaternion.identity);
        ctrl.enabled = true;
        cam.transform.rotation = Quaternion.identity;
        cam.transform.rotation = transform.rotation;
        moveCS.ResetVelocityVertical();
    }
    public void ResetChar(Transform spawnPoint)
    {
        Debug.Log(transform.position);
        ctrl.enabled = false;
        transform.SetPositionAndRotation(spawnPoint.position, spawnPoint.rotation);
        ctrl.enabled = true;
        cam.transform.rotation = spawnPoint.rotation;
        cam.transform.rotation = transform.rotation;
        moveCS.ResetVelocityVertical();
    }
}