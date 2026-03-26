using Interfaces;
using System.Collections;
using UnityEngine;

public class MagicMissile : MonoBehaviour
{
    private GameObject target;
    public float speed = 10f;
    public float rotationSpeed = 200f;
    public float damage = 20f;
    [SerializeField] private float lifetime = 5f;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100f))
        {
            if (hit.collider.CompareTag("Enemy"))
            {
                target = hit.collider.gameObject;
            }
        }
        if (target == null)
        {
            target = FindClosestEnemy();
        }


        Destroy(gameObject, lifetime);
    }

    void FixedUpdate()
    {
        if (target != null)
        {
            Vector3 direction = (target.transform.position - transform.position).normalized;
            rb.AddForce(direction * speed);
        }
        else {
            rb.AddForce(transform.forward * speed);
        }
    }

    public void SetTarget(GameObject newTarget)
    {
        target = newTarget;
    }

    GameObject FindClosestEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject closestEnemy = null;
        float minDistance = Mathf.Infinity;

        foreach (GameObject enemy in enemies)
        {
            float distance = Vector3.Distance(transform.position, enemy.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                closestEnemy = enemy;
            }
        }
        if (closestEnemy == null) return null;

        return closestEnemy;
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