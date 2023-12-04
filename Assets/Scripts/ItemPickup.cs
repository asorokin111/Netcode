using FishNet.Connection;
using FishNet.Object;
using UnityEngine;

public class ItemPickup : NetworkBehaviour
{
    public delegate void GolfClubPickupAction();
    public static event GolfClubPickupAction OnClubPickedUp;

    public delegate void GolfClubDropAction();
    public static event GolfClubDropAction OnClubDropped;

    [Header("Pickup inputs")]
    [SerializeField]
    private KeyCode _pickupButton = KeyCode.E;
    [SerializeField]
    private KeyCode _dropButton = KeyCode.Q;

    [Header("Raycast settings")]
    [SerializeField]
    private float _pickupDistance = 5.0f;
    [SerializeField]
    private LayerMask _pickupLayer;

    [SerializeField]
    private Transform _pickupTransform;
    private bool _hasObjectInHand;
    private GameObject _heldObject;
    private Camera _cam;

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
        if (Input.GetKeyDown(_pickupButton))
        {
            Pickup();
        }

        if (Input.GetKeyDown(_dropButton))
        {
            DropHeldObjectServer();
        }
    }

    private void Pickup()
    {
        if (_hasObjectInHand) return;
        if (Physics.Raycast(_cam.transform.position, _cam.transform.forward, out RaycastHit hit, _pickupDistance, _pickupLayer))
        {
            TakeObjectServer(hit.transform.gameObject, _pickupTransform.position, _pickupTransform.rotation, gameObject);
        }
    }

    [ServerRpc (RequireOwnership = false)]
    private void DropHeldObjectServer()
    {
        if (!_hasObjectInHand) return;
        DropHeldObjectObserver();
    }

    [ObserversRpc]
    private void DropHeldObjectObserver()
    {
        _heldObject.transform.SetParent(null);
        if (_heldObject.TryGetComponent(out Rigidbody rb))
        {
            rb.isKinematic = false;
        }
        if (_heldObject.CompareTag("GolfClub"))
        {
            OnClubDropped?.Invoke();
        }
        _heldObject = null;
        _hasObjectInHand = false;
    }

    [ServerRpc (RequireOwnership = false)]
    private void TakeObjectServer(GameObject ob, Vector3 pos, Quaternion rotation, GameObject player)
    {
        TakeObjectObserver(ob, pos, rotation, player);
    }

    [ObserversRpc]
    private void TakeObjectObserver(GameObject ob, Vector3 pos, Quaternion rotation, GameObject player)
    {
        ob.transform.position = pos;
        ob.transform.rotation = rotation;
        ob.transform.SetParent(player.transform);
        if (ob.TryGetComponent(out Rigidbody rb))
        {
            rb.isKinematic = true;
        }
        _heldObject = ob;
        _hasObjectInHand = true;
        if (ob.CompareTag("GolfClub"))
        {
            OnClubPickedUp?.Invoke();
        }
    }
}
