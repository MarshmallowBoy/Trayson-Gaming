using UnityEngine;
using UnityEngine.AI;

public class WanderingEnemyWithChase : MonoBehaviour
{
    public float wanderRadius = 10f;     // Radius within which the enemy can wander
    public float wanderTimer = 2f;       // Time before the enemy chooses a new random position
    public float wanderSpeed = 2f;       // Speed of the enemy when wandering
    public float chaseSpeed = 5f;        // Speed of the enemy when chasing the player
    public float detectionRange = 10f;   // Range at which the enemy detects players
    public float losePlayerRange = 15f;  // Range at which the enemy stops chasing the player
    public string playerTag = "Player";  // Tag used to identify players in the scene

    private NavMeshAgent navAgent;        // Reference to the NavMeshAgent component
    private Vector3 initialPosition;      // Starting position of the enemy
    private float timer;                  // Timer to wait before picking a new destination
    private float idleTimer;              // Timer for idle state
    private bool isIdle;                  // Flag to check if the enemy is idle
    private bool isChasing;               // Flag to check if the enemy is chasing a player
    private GameObject targetPlayer;      // The current target player that the enemy is chasing

    void Start()
    {
        navAgent = GetComponent<NavMeshAgent>();
        initialPosition = transform.position;
        navAgent.speed = wanderSpeed;  // Set the initial speed to wander speed
        timer = wanderTimer;           // Start the timer for wandering
    }

    void Update()
    {
        if (isChasing)
        {
            // Handle chasing behavior
            ChasePlayer();
        }
        else
        {
            // Handle wandering behavior
            Wander();
        }

        // Check if any players are within detection range and find the nearest one
        CheckForPlayers();
    }

    // Method to find the nearest player and set it as the target
    void CheckForPlayers()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag(playerTag);
        GameObject nearestPlayer = null;
        float nearestDistance = Mathf.Infinity;

        foreach (GameObject player in players)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

            // Check if player is within detection range and has line of sight
            if (distanceToPlayer <= detectionRange && CanSeePlayer(player))
            {
                if (distanceToPlayer < nearestDistance)
                {
                    nearestDistance = distanceToPlayer;
                    nearestPlayer = player;
                }
            }
        }

        // If a nearest player is found, start chasing them
        if (nearestPlayer != null)
        {
            targetPlayer = nearestPlayer;
            isChasing = true;
            navAgent.speed = chaseSpeed;  // Switch to chase speed
        }
        else if (targetPlayer != null && Vector3.Distance(transform.position, targetPlayer.transform.position) > losePlayerRange)
        {
            // If the player is out of chase range, stop chasing and return to wandering
            targetPlayer = null;
            isChasing = false;
            navAgent.speed = wanderSpeed; // Return to wander speed
            MoveToPatrolPoint();          // Resume wandering
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

    // Method for chasing the target player
    void ChasePlayer()
    {
        if (targetPlayer != null)
        {
            // Move directly towards the target player
            navAgent.SetDestination(targetPlayer.transform.position);
        }
    }

    // Method for wandering
    void Wander()
    {
        if (isIdle)
        {
            // Handle idle behavior
            idleTimer -= Time.deltaTime;
            if (idleTimer <= 0)
            {
                isIdle = false; // Stop being idle and move to a new position
                PickNewDestination();
            }
        }
        else
        {
            // Handle wandering behavior
            timer -= Time.deltaTime;

            // If the timer runs out, pick a new random destination
            if (timer <= 0)
            {
                timer = wanderTimer;  // Reset the timer
                PickNewDestination();
            }

            // Move towards the destination
            if (navAgent.remainingDistance <= navAgent.stoppingDistance)
            {
                // If the enemy has reached its destination, go idle for a random time
                StartIdle();
            }
        }
    }

    // Method to pick a random destination within the wander radius
    void PickNewDestination()
    {
        // Random point within a sphere of wanderRadius around the initial position
        Vector3 randomDirection = Random.insideUnitSphere * wanderRadius;
        randomDirection += initialPosition;

        // Ensure that the new destination is on the NavMesh
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDirection, out hit, wanderRadius, NavMesh.AllAreas))
        {
            navAgent.SetDestination(hit.position); // Set the new destination
        }
    }

    // Start idle behavior by setting a random idle time
    void StartIdle()
    {
        isIdle = true;
        idleTimer = Random.Range(1f, 3f);  // Set random idle time between 1 and 3 seconds
    }

    // Method to move the enemy back to its original patrol area if the player is not in range
    void MoveToPatrolPoint()
    {
        // Pick a random point in the original patrol radius
        PickNewDestination();
    }
}
