using System.Collections.Generic;
using Interfaces;
using UnityEngine;

// Attack script should be placed on all attack hitboxes.
public class Attack : AttackTemplate
{
    public List<IDamageable> Damageables { get; } = new();

    void Start()
    {
        transform.localScale = Vector3.one * scale;
    }

    public void OnTriggerStay(Collider other)
    {
        // Add the damageable object when it enters the hitbox.
        var damageable = other.GetComponent<IDamageable>();
        if (damageable != null && !Damageables.Contains(damageable) && !damageable.IsPlayer) Damageables.Add(damageable);
    }

    public void OnTriggerExit(Collider other)
    {
        // Remove the damageable object when it leaves the hitbox.
        var damageable = other.GetComponent<IDamageable>();
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
