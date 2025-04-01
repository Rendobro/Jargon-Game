using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using cm = CheckpointManager;
public class VoidScript : MonoBehaviour
{
    public static event Action<Transform> OnPlayerHitVoid;

    private void OnTriggerEnter(Collider hit)
    {
        if (hit.CompareTag("Player"))
        {
            OnPlayerHitVoid?.Invoke(hit.transform);
        }
    }
    
}
