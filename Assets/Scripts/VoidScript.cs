using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoidScript : MonoBehaviour
{
    public Transform checkpoint;
    private Transform actualCheckpoint;
    public CharacterController player;
    private PlayerResetScript prs;
    // Start is called before the first frame update
    void Start()
    {
        prs = player.GetComponent<PlayerResetScript>();
    }

    // Update is called once per frame
    void Update()
    {
        actualCheckpoint = checkpoint;
    }

    private void OnTriggerEnter(Collider hit)
    {
        if (hit.CompareTag("Player"))
        {
            if (actualCheckpoint != null)
            prs.ResetChar(actualCheckpoint);
            else
            prs.ResetChar();
        }
    }

    public void ChangeCheckpoint(Transform newCheckpoint)
    {
        actualCheckpoint.position = newCheckpoint.position;
    }
    public void ChangeCheckpoint(Vector3 newCheckpointPos)
    {
        actualCheckpoint.position = newCheckpointPos;
    }
}
