using Interfaces;
using UnityEngine;
using UnityEngine.UI;

public class HealthNoNetcode : MonoBehaviour, IDamageable
{
    [SerializeField] float maxHealth = 100f;
    [SerializeField] Slider healthSlider;
    [SerializeField] bool isPlayer;
    public bool IsPlayer => isPlayer;
    public bool invincible;

    float currentHealth;

    public float CurrentHealth => currentHealth;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float amount)
    {
        if (invincible) return;

        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);

        if (healthSlider) healthSlider.value = currentHealth / maxHealth;

        if (currentHealth <= 0f) Destroy(gameObject);
    }
    
    public void Heal(float amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);
    }
}