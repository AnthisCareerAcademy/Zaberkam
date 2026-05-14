using UnityEngine;
using Unity.Netcode;

// Attack script should be placed on all attack hitboxes.
public abstract class AttackTemplate : NetworkBehaviour
{
    [SerializeField] protected int damage;
    public float scale = 1f;
    
    // Call this to perform an attack.
    public abstract void DoAttack(int bonus = 0, float multiplier = 1f, Vector3? direction = null);
}
