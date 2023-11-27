using FishNet;
using FishNet.Broadcast;
using FishNet.Connection;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;

public class PlayerHealth : NetworkBehaviour
{
    [SyncVar]
    public int health = 100;

    public override void OnStartClient()
    {
        base.OnStartClient();
        if (!IsOwner)
        {
            enabled = false;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            AdjustHealth(-10);
        }
    }

    public void AdjustHealth(int diff)
    {
        UpdateHealth(this, diff);
    }

    [ServerRpc]
    private void UpdateHealth(PlayerHealth script, int diff)
    {
        script.health += diff;
    }
}
