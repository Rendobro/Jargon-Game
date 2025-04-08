using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using cm = CheckpointManager;
public class VoidScript : MonoBehaviour
{
    private void OnTriggerEnter(Collider hit)
    {
        if (hit.CompareTag("Player"))
        {
            EventManager.Instance.OnPlayerHitVoid?.Invoke(hit.transform);
        }
    }
    
}
