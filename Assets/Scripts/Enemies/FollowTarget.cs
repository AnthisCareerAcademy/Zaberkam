using UnityEngine;

public class FollowTarget : MonoBehaviour
{
    public float speed = 2f;
    public float detectDistance = 10f;

    private Rigidbody rb;
    
    private GameObject target;
    private RaycastHit hit;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        if (!rb) Debug.LogError("No Rigidbody found");
        
        float scale = transform.localScale.x;
        
        speed *= scale;
        detectDistance *= scale;
    }

    void FixedUpdate()
    {
        if (target)
        {
            transform.LookAt(target.transform);
            rb.AddForce(transform.forward * (speed * Time.deltaTime), ForceMode.Impulse);
        }
        else
        {
            if (Physics.SphereCast(transform.position, detectDistance, transform.forward, out hit))
            {
                Debug.DrawLine(transform.position, hit.point, Color.red);
                GameObject newTarget = hit.collider.gameObject;
                print("yay target! " + newTarget.name);
                // I had to compare to a boolean because the health component might not exist....
                if (newTarget.GetComponent<Health>()?.IsPlayer == true) target = newTarget;
            }
        }
    }
}
