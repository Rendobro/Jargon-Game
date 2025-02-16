using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnScript : MonoBehaviour
{
    public GameObject player;
    private PlayerResetScript prs;
    // Start is called before the first frame update
    void Start()
    {
        prs = player.GetComponent<PlayerResetScript>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Reset"))
        {
            prs.ResetChar(transform);
        }
    }
}
