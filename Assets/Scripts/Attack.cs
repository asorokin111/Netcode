using FishNet.Object;
using UnityEngine;

public class Attack : NetworkBehaviour
{
    public int damage;
    public float attackCooldown;
    private float _attackTimer;

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
        if (Input.GetButtonDown("Fire1") && _attackTimer <= 0)
        {
            AttackServer(damage, Camera.main.transform.position, Camera.main.transform.forward);
            _attackTimer = attackCooldown;
        }
        if (_attackTimer > 0) _attackTimer -= Time.deltaTime;
    }

    [ServerRpc (RequireOwnership = false)]
    private void AttackServer(int damage, Vector3 position, Vector3 direction)
    {
        if (Physics.Raycast(position, direction, out var hit, 5f)
            && hit.transform.CompareTag("Player"))
        {
            var enemyHp = hit.transform.GetComponent<PlayerHealth>();
            enemyHp.AdjustHealth(-damage);
        }
    }
}
