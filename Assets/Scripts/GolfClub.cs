using FishNet.Connection;
using FishNet.Object;
using UnityEngine;

public class GolfClub : NetworkBehaviour
{
    public delegate void HitForceChangedAction(float maxForce, float currentForce, float initialForce);
    public static event HitForceChangedAction OnHitForceChanged;

    [SerializeField]
    private GameObject _aimingDecalProjector;
    [Header("Hit variables")]
    [SerializeField]
    private Transform _hitDirection;
    [SerializeField]
    private LayerMask _golfMask;
    [SerializeField]
    private float _hitDistance;
    [SerializeField]
    private float _maxHitForce;
    [SerializeField]
    [Tooltip("The amount of force added when the attack button is held")]
    private float _forceAdded = 2;
    [SerializeField]
    private float _initialHitForce;
    [SerializeField]
    private float _forceMultiplier = 5.0f;
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
            _aimingDecalProjector.SetActive(true);
            IncreaseHitForce();
        }
        if (Input.GetButtonUp("Fire1") && isClubHeld)
        {
            Hit(_hitForce * _forceMultiplier);
            ResetHitForce();
            _aimingDecalProjector.SetActive(false);
        }
    }

    private void IncreaseHitForce()
    {
        if (_hitForce < _maxHitForce)
            _hitForce += _forceAdded * Time.deltaTime;
        OnHitForceChanged?.Invoke(_maxHitForce, _hitForce, _initialHitForce);
    }

    private void ResetHitForce()
    {
        _hitForce = _initialHitForce;
        OnHitForceChanged?.Invoke(_maxHitForce, _hitForce, _initialHitForce);
    }

    private void Hit(float force)
    {
        if (Physics.Raycast(_cam.transform.position, _cam.transform.forward, out RaycastHit hit, _hitDistance, _golfMask))
        {
            var target = hit.collider;
            if (target.CompareTag("GolfBall"))
            {
                HitBallServer(target.gameObject, force);
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
        Vector3 eulers = _hitDirection.eulerAngles;
        eulers.x = 0;
        eulers.z = 0;
        _hitDirection.eulerAngles = eulers;
        var ballScript = ball.GetComponent<GolfBall>();
        ballScript.lastHitter = Owner;
        ballScript.lastHitPosition = ball.transform.position;
        var rbForce = _hitDirection.forward * force;
        rb.AddForce(rbForce);
    }
}
