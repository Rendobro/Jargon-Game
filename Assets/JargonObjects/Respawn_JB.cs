using prm = PlayerResetManager;
using UnityEngine;

public class Respawn_JB : JargonBlock
{
    void OnTriggerStay(Collider hit)
    {
        prm.Instance.ResetChar();
    }
}
