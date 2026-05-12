using UnityEngine;

public class FollowTarget : MonoBehaviour
{
    public float speed = 2f;
    public float detectDistance = 5f;

    private CharacterController cc;
    
    private GameObject target;
    private RaycastHit hit;

    void Awake()
    {
        cc = GetComponent<CharacterController>();
        if (!cc) Debug.LogError("No CharacterController found");
        
        float scale = transform.localScale.x;
        
        speed *= scale;
        
        cc.minMoveDistance *= scale;
        cc.skinWidth = 0.05f * scale;
        cc.stepOffset *= scale;
    }

    void FixedUpdate()
    {
        if (target)
        {
            transform.LookAt(target.transform);
            cc.SimpleMove(transform.forward * (speed * Time.deltaTime));
        }
        else
        {
            if (Physics.SphereCast(transform.position, detectDistance, transform.forward, out hit))
            {
                GameObject newTarget = hit.collider.gameObject;
                // I had to compare to a boolean because the health component might not exist....
                if (newTarget.GetComponent<Health>()?.IsPlayer == true) target = newTarget;
            }
        }
    }
}
