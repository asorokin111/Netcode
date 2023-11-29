using FishNet;
using FishNet.Broadcast;
using FishNet.Connection;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using TMPro;
using UnityEngine;

public class PlayerHealth : NetworkBehaviour
{
    [SyncVar]
    public int health = 100;
    [SerializeField]
    private TextMeshProUGUI _healthText;

    public override void OnStartClient()
    {
        base.OnStartClient();
        if (!IsOwner)
        {
            enabled = false;
        }
    }

    private void Start()
    {
        _healthText = GameObject.Find("HealthText").GetComponent<TextMeshProUGUI>();
        _healthText.text = (health / 10).ToString();
    }

    public void AdjustHealth(int diff)
    {
        UpdateHealth(this, diff);
        _healthText.text = health.ToString();
    }

    [ServerRpc]
    private void UpdateHealth(PlayerHealth script, int diff)
    {
        script.health += diff;
    }

    public void Die()
    {

    }
}
