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
        int checkpointNum = int.Parse(hit.gameObject.transform.parent.gameObject.name[^1]+"");
        // checks if current checkpoint is already stored or if it's stored but doesn't match where the player actually respawns
        if ((PlayerPrefs.GetInt("checkpoint") != checkpointNum) || ((PlayerPrefs.GetInt("checkpoint") == checkpointNum) && vs.GetActualCheckpoint().position != hit.gameObject.transform.parent.position))
        {
            PlayerPrefs.SetInt("checkpoint", checkpointNum);
            vs.ChangeCheckpoint(hit.transform.position);
        }
        }
    }
}
