using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnScript : MonoBehaviour
{
    private PlayerResetScript prs;
    // Start is called before the first frame update
    void Start()
    {
        prs = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerResetScript>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Reset"))
        {
            prs.ResetChar(transform);
        }
        if (Input.GetButtonDown("HardReset"))
        {
            prs.HardResetChar();
        }
    }
}
