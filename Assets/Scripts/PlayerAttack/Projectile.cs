using Interfaces;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float damage;
    public Rigidbody rb;

    [SerializeField] float lifetime = 5f;

    public virtual void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null) Debug.LogError($"No Rigidbody found on {name}");
        
        if (lifetime > 0) Destroy(gameObject, lifetime);
    }

    public void OnCollisionEnter(Collision collision)
    {
        ContactPoint contact = collision.contacts[0];
        // Prevent projectiles from colliding with each other.
        if (contact.otherCollider.GetType() == contact.thisCollider.GetType()
            || contact.otherCollider.gameObject.CompareTag("Player")) return;
        
        IDamageable damageable = contact.otherCollider.GetComponent<IDamageable>();

        print($"Doing {damage} to {contact.otherCollider.name}");

        damageable?.TakeDamage(damage);
        Destroy(gameObject);
    }

    public void Fire(float speed)
    {
        rb.AddForce(transform.forward * speed, ForceMode.Impulse);
    }
}
