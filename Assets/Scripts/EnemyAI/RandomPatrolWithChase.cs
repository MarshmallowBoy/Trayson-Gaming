using UnityEngine;
using UnityEngine.AI;

public class RandomPatrolWithChase : MonoBehaviour
{
    public Transform[] patrolPoints;      // Array of waypoints for patrol
    public float waitTimeAtWaypoint = 2f; // Time to wait at each waypoint
    public float patrolSpeed = 2f;        // Speed of the enemy during patrol
    public float chaseSpeed = 5f;         // Speed of the enemy when chasing the player
    public float detectionRange = 10f;    // Range at which the enemy detects the player
    public float losePlayerRange = 15f;   // Range at which the enemy stops chasing the player
    public string playerTag = "Player";   // Tag to identify the player(s)

    private NavMeshAgent navAgent;        // NavMeshAgent for moving the enemy
    private int currentPatrolIndex;       // Current waypoint index
    private bool isWaiting;               // Is the enemy waiting at a waypoint
    private bool isChasing;               // Is the enemy chasing the player
    private float waitTimer;              // Timer for waiting at waypoints
    private GameObject targetPlayer;      // The current target player that the enemy is chasing

    void Start()
    {
        navAgent = GetComponent<NavMeshAgent>();
        navAgent.speed = patrolSpeed; // Set the patrol speed by default

        if (patrolPoints.Length == 0)
        {
            Debug.LogError("No patrol points assigned.");
            return;
        }

        currentPatrolIndex = Random.Range(0, patrolPoints.Length);
        MoveToNextPatrolPoint();
    }

    void Update()
    {
        if (isChasing)
        {
            // If the enemy is chasing, move towards the player
            ChasePlayer();
        }
        else
        {
            // If the enemy is patrolling, check if it's time to wait or move to the next point
            Patrol();
        }

        // Check if there is a player within detection range and line of sight
        CheckPlayerDetection();
    }

    void CheckPlayerDetection()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag(playerTag);
        GameObject nearestPlayer = null;
        float nearestDistance = Mathf.Infinity;

        // Find the nearest player
        foreach (GameObject player in players)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

            // Check if player is within detection range and line of sight
            if (distanceToPlayer <= detectionRange && CanSeePlayer(player))
            {
                if (distanceToPlayer < nearestDistance)
                {
                    nearestDistance = distanceToPlayer;
                    nearestPlayer = player;
                }
            }
        }

        if (nearestPlayer != null)
        {
            // Player found, start chasing
            targetPlayer = nearestPlayer;
            isChasing = true;
            navAgent.speed = chaseSpeed; // Increase speed to chase
        }
        else if (targetPlayer != null && Vector3.Distance(transform.position, targetPlayer.transform.position) > losePlayerRange)
        {
            // Player is out of the lose range, stop chasing and resume patrolling
            targetPlayer = null;
            isChasing = false;
            navAgent.speed = patrolSpeed; // Reset to patrol speed
            MoveToNextPatrolPoint();
        }
    }

    // Check if there is a clear line of sight to the player
    bool CanSeePlayer(GameObject player)
    {
        Vector3 directionToPlayer = player.transform.position - transform.position;
        RaycastHit hit;

        // Cast a ray to check if there are any obstacles between the enemy and the player
        if (Physics.Raycast(transform.position, directionToPlayer, out hit, detectionRange))
        {
            if (hit.collider.CompareTag(playerTag)) // If the ray hits a player, we have line of sight
            {
                return true;
            }
        }

        return false;
    }

    void ChasePlayer()
    {
        if (targetPlayer != null)
        {
            // Move directly towards the player
            navAgent.SetDestination(targetPlayer.transform.position);
        }
    }

    void Patrol()
    {
        if (navAgent.remainingDistance <= navAgent.stoppingDistance && !isWaiting)
        {
            // Wait at the current patrol point for a random time
            isWaiting = true;
            waitTimer = waitTimeAtWaypoint;
        }

        if (isWaiting)
        {
            waitTimer -= Time.deltaTime;

            if (waitTimer <= 0)
            {
                isWaiting = false;
                MoveToNextPatrolPoint();
            }
        }
    }

    void MoveToNextPatrolPoint()
    {
        if (patrolPoints.Length == 0)
            return;

        // Pick a random patrol point
        currentPatrolIndex = Random.Range(0, patrolPoints.Length);

        // Set the destination to the selected patrol point
        navAgent.SetDestination(patrolPoints[currentPatrolIndex].position);
    }
}
