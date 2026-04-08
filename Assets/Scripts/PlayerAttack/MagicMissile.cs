using Interfaces;
using UnityEngine;

public class MagicMissile : Projectile
{
    private GameObject target;
    
    // THIS MUST BE SET IN PREFAB--the normal speed setter doesn't work.
    public float speed = 10f;
    [SerializeField] private float lifetime = 5f;

    void Start()
    {
        FindTarget();

        Destroy(gameObject, lifetime);
    }

    void FixedUpdate()
    {
        if (target)
        {
            Vector3 direction = (target.transform.position - transform.position).normalized;
            rb.AddForce(direction * speed);
        }
        else
        {
            rb.AddForce(transform.forward * speed);
            target = FindTarget();
        }
    }

    GameObject FindTarget()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;
        
        if (Physics.Raycast(ray, out hit, 100f))
        {
            if (hit.collider)
            {
                return hit.collider.gameObject;
            }
        }

        return null;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
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