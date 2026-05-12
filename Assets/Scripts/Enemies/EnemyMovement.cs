using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    public float detectionRange = 15f;
    public float attackRange = 2f;
    public float moveSpeed = 5f;
    public float repathRate = 0.2f;
    public LayerMask wallMask;

    private Transform player;
    private float repathTimer;
    private Vector3 targetPosition;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    void Update()
    {
        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= detectionRange && CanSeePlayer())
        {
            repathTimer -= Time.deltaTime;

            if (repathTimer <= 0f)
            {
                if (player.CompareTag("Player"))
                {
                    targetPosition = player.position;
                }
                else
                {
                    targetPosition = transform.position;
                }

                repathTimer = repathRate;
            }

            if (distance <= attackRange)
            {
                // Stop moving and attack
                // TODO: Add attack logic here
            }
            else
            {
                MoveTowardsTarget();
            }
        }
    }

    void MoveTowardsTarget()
    {
        Vector3 direction = (targetPosition - transform.position).normalized;

        // Move
        transform.position += direction * moveSpeed * Time.deltaTime;

        // Optional: rotate toward player
        if (direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, 10f * Time.deltaTime);
        }
    }

    bool CanSeePlayer()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        float distance = Vector3.Distance(transform.position, player.position);

        if (!Physics.Raycast(transform.position, direction, distance, wallMask))
        {
            return true;
        }

        return false;
    }
}