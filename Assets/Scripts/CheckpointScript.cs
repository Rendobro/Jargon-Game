using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CheckpointScript : MonoBehaviour
{
    public VoidScript vs;

    private void OnTriggerEnter(Collider hit)
    {
        if (hit.gameObject.layer == 7)
        {
        int checkpointNum = int.Parse(hit.gameObject.transform.parent.gameObject.name[^1]+"");
        if (SceneManager.GetActiveScene().isLoaded && !vs.GetActualCheckpoint().position.Equals(hit.transform.position))
        {
            PlayerPrefs.SetInt("checkpoint", checkpointNum);
            vs.ChangeCheckpoint(hit.transform.position);
        }
        }
    }
}
