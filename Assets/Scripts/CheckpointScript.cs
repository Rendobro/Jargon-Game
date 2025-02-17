using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointScript : MonoBehaviour
{
    private VoidScript vs;
    // Start is called before the first frame update
    void Start()
    {
        vs = GameObject.FindGameObjectWithTag("Void").GetComponent<VoidScript>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider hit)
    {
        if (hit.gameObject.layer == 7)
        {
            Debug.Log("Checkpoint baby! " + hit.transform.position);
            vs.ChangeCheckpoint(hit.transform.position);
        }
    }
}
