using Unity.Netcode;
using UnityEngine;

public class RangedAttack : AttackTemplate
{
    [SerializeField] Projectile projectile;
    [SerializeField] float speed;
    [SerializeField] bool useGravity;

    public override void DoAttack(int bonus = 0, float multiplier = 1F, Vector3? direction = null)
    {
        if (!IsOwner) return;

        direction ??= transform.eulerAngles;

        ShootServerRpc(bonus, multiplier, direction.Value);
    }

    [ServerRpc]
    void ShootServerRpc(int bonus, float multiplier, Vector3 direction)
    {
        Projectile newProjectile = Instantiate(projectile, transform.position, Quaternion.identity);

        GameObject projectileObj = newProjectile.gameObject;
        projectileObj.transform.localScale *= scale;
        newProjectile.transform.eulerAngles = direction;
        newProjectile.damage = (damage + bonus) * multiplier;
        newProjectile.rb.useGravity = useGravity;

        NetworkObject netObj = newProjectile.GetComponent<NetworkObject>();
        netObj.Spawn();

        newProjectile.Fire(speed * scale);
    }
}
