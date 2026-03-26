using System.Collections.Generic;
using Interfaces;
using UnityEngine;

// Attack script should be placed on all attack hitboxes.
public class Attack : AttackTemplate
{
    // Tags that shouldn't be damaged by this attack.
    [SerializeField] bool canDamagePlayer;
    [SerializeField] bool canDamageEnemies;
    public List<IDamageable> Damageables { get; } = new();

    public void OnTriggerStay(Collider other)
    {
        // Add the damageable object when it enters the hitbox.
        if (!canDamagePlayer && other.CompareTag("Player")) return;
        if (!canDamageEnemies && other.CompareTag("Enemy")) return;
        var damageable = other.GetComponentInParent<IDamageable>();
        if (damageable != null && !Damageables.Contains(damageable)) Damageables.Add(damageable);
    }

    public void OnTriggerExit(Collider other)
    {
        // Remove the damageable object when it leaves the hitbox.
        var damageable = other.GetComponentInParent<IDamageable>();
        if (damageable != null && Damageables.Contains(damageable)) Damageables.Remove(damageable);
    }
    
    public override void DoAttack(float multiplier = 1f, Vector3? direction = null)
    {
        // Damage each enemy in the hitbox.
        foreach (var damageable in Damageables)
        {
            // DEBUG: print damage
            print($"Dealing {damage * multiplier} damage to {damageable}");
            damageable?.TakeDamage(damage * multiplier);
        }

        Damageables.Clear();
    }
}
