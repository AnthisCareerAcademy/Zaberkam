using UnityEngine;
using UnityEngine.AI;

public class EnemyMovement : MonoBehaviour
{
    public float detectionRange = 15f;
    public float attackRange = 2f;
    public float repathRate = 0.2f;
    public LayerMask wallMask; // Layer mask for walls/obstacles

    private NavMeshAgent agent;
    private Transform player;
    private float repathTimer;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
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
                if (player.tag == "Player")
                {
                    agent.SetDestination(player.position);
                }
                if (player.tag == "Untagged")
                {
                    agent.ResetPath();
                }
                repathTimer = repathRate;
            }

            if (distance <= attackRange)
            {
                agent.isStopped = true;
                // TODO: Add attack logic here
            }
            else
            {
                agent.isStopped = false;
            }
        }
        else
        {
            agent.ResetPath();
        }
    }

    bool CanSeePlayer()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        float distance = Vector3.Distance(transform.position, player.position);

        // Raycast from enemy to player
        if (!Physics.Raycast(transform.position, direction, distance, wallMask))
        {
            return true; // No obstacles in the way
        }

        return false; // Player is blocked
    }
}