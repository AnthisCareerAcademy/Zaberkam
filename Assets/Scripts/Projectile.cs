using System;
using System.Collections;
using Interfaces;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float damage;
    public Rigidbody rb;
    public float lifetime;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null) Debug.LogError($"No Rigidbody found on {name}");
    }

    public void OnCollisionEnter(Collision collision)
    {
        ContactPoint contact = collision.contacts[0];
        // Prevent projectiles from colliding with each other.
        if (contact.otherCollider.GetType() == contact.thisCollider.GetType()) return;
        
        IDamageable damageable = contact.otherCollider.GetComponent<IDamageable>();

        print($"Doing {damage} to {contact.otherCollider.name}");
        damageable?.TakeDamage(damage);
        
        if (lifetime == 0) Destroy(gameObject);
    }

    public void Fire(float speed)
    {
        rb.AddForce(transform.forward * speed, ForceMode.Impulse);
        if (lifetime > 0) StartCoroutine(Lifetime());
    }

    IEnumerator Lifetime()
    {
        yield return new WaitForSeconds(lifetime);
        Destroy(gameObject);
    }
}
