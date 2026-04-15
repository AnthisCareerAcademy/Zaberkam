using Interfaces;
using UnityEngine;

public class FireBall: Projectile
{
    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player") && !other.isTrigger)
        {
            IDamageable damageable = other.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(damage);
            }
            Destroy(gameObject);
        }
    }
}