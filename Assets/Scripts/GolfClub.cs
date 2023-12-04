using FishNet.Object;
using UnityEngine;
using UnityEngine.Assertions;

public class GolfClub : NetworkBehaviour
{
    [SerializeField]
    private float _hitDistance;
    [SerializeField]
    private float _maxHitForce;
    [SerializeField]
    [Tooltip("The amount of force added when the attack button is held")]
    private float _forceAdded = 2;
    [SerializeField]
    private float _initialHitForce;
    private float _hitForce;
    private bool isClubHeld;
    private Camera _cam;

    private void Awake()
    {
        _hitForce = _initialHitForce;
    }

    private void ClubPickup()
    {
        isClubHeld = true;
        _hitForce = _initialHitForce;
    }

    private void ClubDrop()
    {
        isClubHeld = false;
    }


    private void OnEnable()
    {
        ItemPickup.OnClubPickedUp += ClubPickup;
        ItemPickup.OnClubDropped += ClubDrop;
    }

    private void OnDisable()
    {
        ItemPickup.OnClubPickedUp -= ClubPickup;
        ItemPickup.OnClubDropped -= ClubDrop;
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        if (!IsOwner)
        {
            enabled = false;
        }
        _cam = Camera.main;
    }

    private void Update()
    {
        if (Input.GetButton("Fire1") && isClubHeld)
        {
            if (_hitForce < _maxHitForce)
                _hitForce += _forceAdded * Time.deltaTime;
        }
        if (Input.GetButtonUp("Fire1") && isClubHeld)
        {
            Hit(_hitForce);
            _hitForce = _initialHitForce;
        }
    }

    private void Hit(float force)
    {
        Debug.Log("Attempted hit");
        if (Physics.Raycast(_cam.transform.position, _cam.transform.forward, out RaycastHit hit, _hitDistance))
        {
            var target = hit.collider;
            if (target.CompareTag("Player"))
            {
                HitPlayerServer(target.gameObject, force);
                Debug.Log("Player hit");
            }
            else if (target.CompareTag("GolfBall"))
            {
                HitBallServer(target.gameObject, force);
                Debug.Log("Ball hit");
            }
        }
    }

    [ServerRpc (RequireOwnership = false)]
    private void HitBallServer(GameObject ball, float force)
    {
        HitBallObserver(ball, force);
    }

    [ObserversRpc]
    private void HitBallObserver(GameObject ball, float force)
    {
        if (!ball.TryGetComponent(out Rigidbody rb)) return;
        var rbForce = _cam.transform.forward * force * 5;
        rb.AddForce(rbForce);
    }

    [ServerRpc (RequireOwnership = false)]
    private void HitPlayerServer(GameObject player, float force)
    {
        HitPlayerObserver(player, force);
    }

    [ObserversRpc]
    private void HitPlayerObserver(GameObject player, float force)
    {
        if (!player.TryGetComponent(out Rigidbody rb)) return;
        var rbForce = _cam.transform.forward * (force * 10f);
        rb.AddForce(rbForce);
    }
}
