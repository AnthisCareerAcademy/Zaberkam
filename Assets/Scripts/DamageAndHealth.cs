using UnityEngine;
using UnityEngine.UI;

public class DamageAndHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    private float currentHealth;

    public Slider healthSlider;

    void Start()
    {
        currentHealth = maxHealth;
        if (healthSlider != null)
            healthSlider.value = 1f;
    }

    private void OnTriggerEnter(Collider other)
    {
        IDamageDealer dealer = FindDamageDealer(other);
        if (dealer == null) return;

        TakeDamage(dealer.Damage);
    }

    private IDamageDealer FindDamageDealer(Collider other)
    {
        // components on the collider's GameObject
        foreach (var mb in other.gameObject.GetComponents<MonoBehaviour>())
            if (mb is IDamageDealer d) return d;

        // walk up parents
        Transform t = other.transform.parent;
        while (t != null)
        {
            foreach (var mb in t.gameObject.GetComponents<MonoBehaviour>())
                if (mb is IDamageDealer d) return d;
            t = t.parent;
        }

        // check children
        foreach (var mb in other.gameObject.GetComponentsInChildren<MonoBehaviour>())
            if (mb is IDamageDealer d) return d;

        return null;
    }

    void TakeDamage(float amount)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);
        if (healthSlider != null)
            healthSlider.value = currentHealth / maxHealth;

        //Death 
        if (currentHealth <= 0f)
            Destroy(gameObject);
    }
}