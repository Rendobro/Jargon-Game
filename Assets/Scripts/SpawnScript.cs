using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnScript : MonoBehaviour 
{
    public PlayerResetScript prs;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        ResetChecks();
    }
    private void ResetChecks()
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
