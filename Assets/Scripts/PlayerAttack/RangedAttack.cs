using Unity.Netcode;
using UnityEngine;

public class RangedAttack : AttackTemplate
{
    [SerializeField] Projectile projectile;
    [SerializeField] float speed;
    [SerializeField] bool useGravity;
    
    public override void DoAttack(float multiplier = 1f, Vector3? direction = null)
    {
        direction ??= transform.eulerAngles;

        ShootServerRpc(direction.Value, multiplier);
    }

    [ServerRpc]
    void ShootServerRpc(Vector3 direction, float multiplier)
    {
        Projectile newProjectile = Instantiate(projectile, transform.position, Quaternion.identity);

        GameObject projectileObj = newProjectile.gameObject;
        projectileObj.transform.localScale *= scale;
        newProjectile.transform.eulerAngles = direction;
        newProjectile.damage = damage * multiplier;
        newProjectile.rb.useGravity = useGravity;

        newProjectile.GetComponent<NetworkObject>();

        newProjectile.Fire(speed * scale);
    }
}
