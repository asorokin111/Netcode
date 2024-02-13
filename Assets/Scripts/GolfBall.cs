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

    public void RespawnAtStart()
    {
        RespawnServer(_respawnPosition);
    }

    public void RespawnAtLastHit()
    {
        RespawnServer(_lastHitPosition);
    }

    [ServerRpc (RequireOwnership = false)]
    private void RespawnServer(Vector3 whereToRespawn)
    {
        RespawnObserver(whereToRespawn);
    }

    [ObserversRpc]
    private void RespawnObserver(Vector3 whereToRespawn)
    {
        lastHitter = null;
        var rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeAll;
        transform.position = whereToRespawn;
        rb.constraints = RigidbodyConstraints.None;
    }

    //private void OnCollisionEnter(Collision collision)
    //{
    //    Debug.Log("Collision started");
    //    if (collision.gameObject.layer != _groundLayer)
    //    {
    //        Debug.Log("Layer not matching the needed layer");
    //        return;
    //    }
    //    Debug.Log("Layer matched");
    //    RespawnAtLastHit();
    //}
}
