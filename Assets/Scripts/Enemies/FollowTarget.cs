using NUnit.Framework.Constraints;
using UnityEngine;

public class FollowTarget : MonoBehaviour
{
    public float speed = 15f;
    public float turnSpeed = 5f;
    public float detectRadius = 5f;
    public float followDistance = 1f;
    public int maxChecks = 10;
    [SerializeField] LayerMask layerMask;

    private Rigidbody rb;
    
    private Transform target;
    private Collider[] results;

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
    }

    void FixedUpdate()
    {
        if (target)
        {
            Vector3 lookPoint = target.position - transform.position;
            Quaternion rotation = Quaternion.LookRotation(lookPoint, Vector3.up);
            rb.rotation = Quaternion.Slerp(rb.rotation, rotation, Time.deltaTime * turnSpeed);
            
            print(lookPoint.sqrMagnitude);
            
            if (lookPoint.sqrMagnitude > followDistance)
            {
                rb.AddForce(transform.forward * (speed * Time.deltaTime), ForceMode.Impulse);
            }
            else
            {
                // add damage here
            }
        }
        else
        {
            Physics.OverlapSphereNonAlloc(transform.position, detectRadius, results, layerMask);

            foreach (Collider col in results)
            {
                if (col.gameObject.GetComponent<Health>()?.IsPlayer == true)
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
