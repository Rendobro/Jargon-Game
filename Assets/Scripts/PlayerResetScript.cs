using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerResetScript : MonoBehaviour
{

    public Transform worldSpawn;
    public VoidScript vs;
    public MovementScript moveCS;
    public CharacterController ctrl;
    public Rigidbody rb;
    public Camera cam;
    public void ResetChar()
    {
        ctrl.enabled = false;
        transform.SetPositionAndRotation(worldSpawn.position, Quaternion.identity);
        ctrl.enabled = true;
        cam.transform.rotation = Quaternion.identity;
        moveCS.ResetVelocityVertical();
    }
    public void ResetChar(Transform spawnPoint)
    {
        ctrl.enabled = false;
        transform.SetPositionAndRotation(spawnPoint.position, spawnPoint.rotation);
        ctrl.enabled = true;
        cam.transform.rotation = spawnPoint.rotation;
        moveCS.ResetVelocityVertical();
    }
    public void ResetChar(Vector3 spawnPointPos)
    {
        ctrl.enabled = false;
        transform.position = spawnPointPos;
        transform.localRotation = Quaternion.identity;
        ctrl.enabled = true;
        cam.transform.localRotation = Quaternion.identity;
        moveCS.ResetVelocityVertical();
    }

    public void HardResetChar()
    {
        PlayerPrefs.SetInt("checkpoint",0);
        ctrl.enabled = false;
        transform.SetPositionAndRotation(worldSpawn.position, worldSpawn.rotation);
        ctrl.enabled = true;
        cam.transform.rotation = worldSpawn.rotation;
        moveCS.ResetVelocityVertical();
        vs.ChangeCheckpoint(worldSpawn.position);
    }
}