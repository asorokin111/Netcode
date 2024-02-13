using FishNet.Connection;
using FishNet.Object;
using UnityEngine;

public class GolfBall : NetworkBehaviour
{
    public NetworkConnection lastHitter;
    [SerializeField]
    private LayerMask _groundLayer;
    private Vector3 _lastHitPosition;
    private Vector3 _respawnPosition;

    private void OnEnable()
    {
        GolfClub.OnBallHit += RegisterBallHit;
    }

    private void OnDisable()
    {
        GolfClub.OnBallHit -= RegisterBallHit;
    }

    public override void OnStartClient()
    {
        if (!IsOwner) enabled = false;
        _respawnPosition = transform.position;
    }

    private void RegisterBallHit(NetworkConnection hitter)
    {
        _lastHitPosition = transform.position;
        lastHitter = hitter;
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

    // Soft respawn - respawns the ball at the location of last hit
    [ServerRpc (RequireOwnership = false)]
    public void SoftRespawnServer()
    {
        SoftRespawnObserver();
    }

    [ObserversRpc]
    private void SoftRespawnObserver()
    {
        // Basically the same as normal respawn, but with different position. Should use the same function for both honestly
        lastHitter = null;
        var rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeAll;
        transform.position = _lastHitPosition;
        rb.constraints = RigidbodyConstraints.None;
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Collision started");
        if (collision.gameObject.layer != _groundLayer)
        {
            Debug.Log("Layer not matching the needed layer");
            return;
        }
        Debug.Log("Layer matched");
        SoftRespawnObserver();
    }
}
