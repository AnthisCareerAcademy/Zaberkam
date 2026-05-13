using UnityEngine;

public class EnemyMove : MonoBehaviour
{
    [Header("Attacking")]
    public float damage = 10f;
    public float cooldown = 5f;
    
    [Header("Movement")]
    public float speed = 10f;
    public float turnSpeed = 5f;
    
    [Header("Player Detection")]
    public float detectRadius = 5f;
    public float followDistance = 1f;
    public int maxChecks = 10;
    
    [SerializeField] LayerMask layerMask;

    private Rigidbody rb;
    
    private Transform target;
    private Health targetHealth;
    private Collider[] results;

    private float fdSquared;

    private float time;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (!rb) Debug.LogError("No Rigidbody found");
        
        results = new Collider[maxChecks];
        
        float scale = transform.localScale.x;

        rb.mass *= scale;
        
        speed *= scale;
        detectRadius *= scale;
        followDistance *= scale;

        fdSquared = followDistance * followDistance;  // Square it to avoid unintended behavior
    }

    void Update()
    {
        time -= Time.deltaTime;
        
        if (target)
        {
            Vector3 lookPoint = target.position - transform.position;
            Quaternion rotation = Quaternion.LookRotation(lookPoint, Vector3.up);
            rotation.x = 0f;
            rotation.z = 0f;
            
            rb.rotation = Quaternion.Slerp(rb.rotation, rotation, Time.deltaTime * turnSpeed);
            
            if (lookPoint.sqrMagnitude > fdSquared)
            {
                rb.AddForce(transform.forward * (speed * Time.deltaTime), ForceMode.Impulse);
            }
            else if (time <= 0)
            {
                targetHealth.TakeDamage(damage);
                time = cooldown;
            }
        }
    }

    void FixedUpdate()
    {
        if (!target)
        {
            Physics.OverlapSphereNonAlloc(transform.position, detectRadius, results, layerMask);

            foreach (Collider col in results)
            {
                targetHealth = col.GetComponent<Health>();
                
                if (targetHealth && targetHealth.IsPlayer)
                {
                    target = col.transform;
                    break;
                }
                Debug.DrawLine(transform.position, col.transform.position, Color.red);
            }
        }
    }
    
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Vector3 pos = transform.position;
        Gizmos.DrawWireSphere(pos, detectRadius);
    }
}
