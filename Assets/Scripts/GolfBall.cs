using FishNet.Connection;
using FishNet.Object;
using UnityEngine;

public class GolfBall : NetworkBehaviour
{
    public NetworkConnection lastHitter;
    private Vector3 _respawnPosition;

    public override void OnStartClient()
    {
        if (!IsOwner) enabled = false;
        _respawnPosition = transform.position;
    }

    [ServerRpc (RequireOwnership = false)]
    public void RespawnServer()
    {
        RespawnObserver();
    }

    [ObserversRpc]
    private void RespawnObserver()
    {
        lastHitter = null;
        var rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeAll;
        transform.position = _respawnPosition;
        rb.constraints = RigidbodyConstraints.None;
    }
}
