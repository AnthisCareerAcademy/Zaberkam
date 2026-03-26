using Interfaces;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;

public class Health : NetworkBehaviour, IDamageable
{
    [SerializeField] float maxHealth = 100f;
    [SerializeField] Slider healthSlider;
    public bool invincible;

    private NetworkVariable<float> _healthTracker = new NetworkVariable<float>();

    public float CurrentHealth => _healthTracker.Value;

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            _healthTracker.Value = maxHealth;
        }

        _healthTracker.OnValueChanged += OnHealthChanged;
        OnHealthChanged(_healthTracker.Value, _healthTracker.Value);
    }

    public override void OnNetworkDespawn()
    {
        _healthTracker.OnValueChanged -= OnHealthChanged;
    }

    private void OnHealthChanged(float oldValue, float newValue)
    {
        if (healthSlider)
            healthSlider.value = newValue / maxHealth;
    }

    public void TakeDamage(float amount)
    {
        TakeDamageServerRpc(Mathf.Abs(amount));
    }

    [ServerRpc(RequireOwnership = false)]
    void TakeDamageServerRpc(float amount)
    {
        if (invincible) return;

        _healthTracker.Value = Mathf.Clamp(_healthTracker.Value - amount, 0f, maxHealth);

        if (_healthTracker.Value <= 0f)
        {
            Die();
        }
    }

    public void Heal(float amount)
    {
        HealServerRpc(Mathf.Abs(amount));
    }

    [ServerRpc(RequireOwnership = false)]
    void HealServerRpc(float amount)
    {
        _healthTracker.Value = Mathf.Clamp(_healthTracker.Value + amount, 0f, maxHealth);
    }

    private void Die()
    {
        GetComponent<NetworkObject>().Despawn();
    }
}