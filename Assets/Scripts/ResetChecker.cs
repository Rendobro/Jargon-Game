using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetChecker : MonoBehaviour
{
    public Transform player;
    public Camera cam;
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log(transform.position);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButton("Reset"))
        {
            player.transform.SetPositionAndRotation(transform.position, transform.rotation);
            cam.transform.localRotation = transform.rotation;
        }

    }
}