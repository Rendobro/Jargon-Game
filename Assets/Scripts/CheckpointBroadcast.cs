using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using cm = CheckpointManager;
public class CheckpointBroadcast : MonoBehaviour
{
    public static event Action<Collider> OnCheckpointHit;
    private void OnTriggerEnter(Collider hit)
    {
        if (hit.gameObject.layer == 7)
        {
            if (SceneManager.GetActiveScene().isLoaded && !cm.Instance.GetCurrentCheckpointTransform().position.Equals(hit.transform.position))
            {
                OnCheckpointHit?.Invoke(hit);
            }
        }
    }
}
