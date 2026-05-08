using UnityEngine;

public class FollowTarget : MonoBehaviour
{
    public float speed = 2f;
    public float detectRadius = 1f;
    public float detectDistance = 1f;

    private Rigidbody rb;
    
    private GameObject target;
    private RaycastHit hit;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (!rb) Debug.LogError("No Rigidbody found");
        
        float scale = transform.localScale.x;
        
        speed *= scale;
        detectRadius *= scale;
        detectDistance *= scale;
    }

    void FixedUpdate()
    {
        if (target)
        {
            print("has target " + target.name);
            transform.LookAt(target.transform);
            rb.AddForce(transform.forward * (speed * Time.deltaTime), ForceMode.Impulse);
        }
        else
        {
            if (Physics.SphereCast(transform.position, detectRadius, transform.forward, out hit, detectDistance))
            {
                Debug.DrawLine(transform.position, hit.point, Color.red);
                GameObject newTarget = hit.collider.gameObject;
                print("yay target! " + newTarget.name);
                // I had to compare to a boolean because the health component might not exist....
                if (newTarget.GetComponent<Health>()?.IsPlayer == true) target = newTarget;
            }
        }
    }
    
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Vector3 pos = transform.position + transform.forward * detectDistance;
        Gizmos.DrawWireSphere(pos, detectRadius);
    }
}
