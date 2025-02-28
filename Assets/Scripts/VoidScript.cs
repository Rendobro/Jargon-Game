using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class VoidScript : MonoBehaviour
{
    [SerializeField] private Transform initialCheckpoint;
    [SerializeField] private CharacterController player;
    [SerializeField] private PlayerResetScript prs;
    private Transform actualCheckpoint;

    // Start is called before the first frame update
    void Start()
    {
        actualCheckpoint = initialCheckpoint;
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
        if (actualCheckpoint != null) actualCheckpoint.position = newCheckpoint.position;
    }
    public void ChangeCheckpoint(Vector3 newCheckpointPos)
    {
        if (actualCheckpoint != null) actualCheckpoint.position = newCheckpointPos;
    }
    public Transform GetActualCheckpoint()
    {
        return actualCheckpoint;
    }
}
