using UnityEngine;

// Attack script should be placed on all attack hitboxes.
public abstract class AttackTemplate : MonoBehaviour
{
    [SerializeField] protected float damage;
    
    // Call this to perform an attack.
    public abstract void DoAttack(float multiplier = 1f, Vector3? direction = null);
}
