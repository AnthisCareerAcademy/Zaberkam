using UnityEngine;

public class RangedAttack : AttackTemplate
{
    [SerializeField] Projectile projectile;
    [SerializeField] float speed;
    [SerializeField] bool useGravity;
    [SerializeField] float lifetime;
    
    public override void DoAttack(float multiplier = 1f, Vector3? direction = null)
    {
        direction ??= transform.eulerAngles;
        
        Projectile newProjectile = Instantiate(projectile, transform.position, Quaternion.identity);
        newProjectile.transform.eulerAngles = direction.Value;
        newProjectile.damage = damage * multiplier;
        newProjectile.lifetime = lifetime;
        newProjectile.rb.useGravity = useGravity;
        
        newProjectile.Fire(speed);
    }
}
