using FishNet.Object;
using FishNet;
using UnityEngine;

public class Windmill : NetworkBehaviour
{
    [SerializeField]
    private float _rotationSpeed;

    private void OnEnable()
    {
        if (IsServer)
            InstanceFinder.TimeManager.OnTick += SpinServer;
    }

    [ServerRpc (RequireOwnership = false)]
    private void SpinServer()
    {
        SpinObserver();
    }

    [ObserversRpc]
    private void SpinObserver()
    {
        transform.Rotate(new Vector3(0, 0, _rotationSpeed));
    }
}
