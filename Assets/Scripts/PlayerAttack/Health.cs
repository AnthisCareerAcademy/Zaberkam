using Interfaces;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;

public class Health : NetworkBehaviour, IDamageable
{
    [SerializeField] float maxHealth = 100f;
    [SerializeField] Slider healthSlider;
    [SerializeField] bool isPlayer;
    public bool IsPlayer => isPlayer;
    public bool invincible;

    private NetworkVariable<float> _healthTracker = new NetworkVariable<float>();

    public float CurrentHealth => _healthTracker.Value;

    private NetworkObject selfNetwork;

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            _healthTracker.Value = maxHealth;
        }

        _healthTracker.OnValueChanged += OnHealthChanged;

        selfNetwork = GetComponent<NetworkObject>();
    }

    private void OnHealthChanged(float oldValue, float newValue)
    {
        if (healthSlider)
            healthSlider.value = newValue / maxHealth;
    }

    public void TakeDamage(float amount)
    {
        TakeDamageServerRpc(amount);
    }

    [ServerRpc(RequireOwnership = false)]
    void TakeDamageServerRpc(float amount)
    {
        if (invincible) return;

        _healthTracker.Value -= amount;
        _healthTracker.Value = Mathf.Clamp(_healthTracker.Value, 0f, maxHealth);

        if (_healthTracker.Value <= 0f)
        {
            selfNetwork.Despawn();
            Destroy(gameObject);
        }
    }

    public void heal (float amount)
    {
        if (IsOwner)
        {
            HealServerRpc(amount);
        }
    }

    [ServerRpc]
    void HealServerRpc(float amount)
    {
        _healthTracker.Value += amount;
        _healthTracker.Value = Mathf.Clamp(_healthTracker.Value, 0f, maxHealth);
    }
}