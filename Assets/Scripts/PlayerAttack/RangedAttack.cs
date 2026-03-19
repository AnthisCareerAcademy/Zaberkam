using UnityEngine;

public class RangedAttack : AttackTemplate
{
    [SerializeField] Projectile projectile;
    [SerializeField] float speed;
    [SerializeField] bool useGravity;
    
    public override void DoAttack(float multiplier = 1f, Vector3? direction = null)
    {
        direction ??= transform.eulerAngles;
        
        Projectile newProjectile = Instantiate(projectile, transform.position, Quaternion.identity);
        GameObject projectileObj = newProjectile.gameObject;
        projectileObj.transform.localScale = Vector3.one * scale;
        newProjectile.transform.eulerAngles = direction.Value;
        newProjectile.damage = damage * multiplier;
        newProjectile.rb.useGravity = useGravity;
        
        newProjectile.Fire(speed * scale);
    }
}
