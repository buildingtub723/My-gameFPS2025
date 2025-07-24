using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(TeamIdentity))]
public class EnemyAI : MonoBehaviour
{
    public enum State { Patrolling, Chasing, Shooting }

    public float detectionRange = 25f;
    public float shootingRange = 12f;
    public float fireRate = 1.5f;
    public float patrolRadius = 10f;

    private Vector3 currentChaseOffset;
    private float nextChaseUpdateTime = 0f;
    public float chaseUpdateInterval = 2f;
    public float chaseOffsetRadius = 4f;

    public GameObject bulletPrefab;
    public Transform firePoint;

    private NavMeshAgent agent;
    private TeamIdentity myTeam;
    private GameObject player;
    [SerializeField] private State currentState;
    private float lastFireTime;
    private Vector3 startPosition;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        myTeam = GetComponent<TeamIdentity>();
        player = GameObject.FindGameObjectWithTag("Player");
        startPosition = transform.position;

        PatrolToNewPoint();
        currentState = State.Patrolling;
    }

    void Update()
    {
        float distToPlayer = Vector3.Distance(transform.position, player.transform.position);

        if (distToPlayer <= detectionRange)
        {
            if (distToPlayer <= shootingRange)
            {
                currentState = State.Shooting;
            }
            else
            {
                currentState = State.Chasing;
            }
        }

        switch (currentState)
        {
            case State.Patrolling:
                HandlePatrolling();
                break;
            case State.Chasing:
                HandleChasing();
                break;
            case State.Shooting:
                HandleShooting();
                break;
        }
    }

    void HandlePatrolling()
    {
        agent.isStopped = false;

        if (!agent.pathPending && agent.remainingDistance < 1f)
        {
            PatrolToNewPoint();
        }
    }

    void HandleChasing()
    {
        agent.isStopped = false;

        if (Time.time >= nextChaseUpdateTime)
        {
            // Pick a new offset point around the player
            Vector2 circle = Random.insideUnitCircle.normalized * chaseOffsetRadius;
            currentChaseOffset = new Vector3(circle.x, 0, circle.y);
            nextChaseUpdateTime = Time.time + chaseUpdateInterval;
        }

        Vector3 targetPosition = player.transform.position + currentChaseOffset;

        // Ensure the point is on the NavMesh
        if (NavMesh.SamplePosition(targetPosition, out NavMeshHit hit, 2f, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }
        else
        {
            // If offset is unreachable, just go to player
            agent.SetDestination(player.transform.position);
        }
    }

    void HandleShooting()
    {
        agent.isStopped = true;

        Vector3 direction = (player.transform.position - firePoint.position).normalized;
        direction.y = 0;
        transform.rotation = Quaternion.LookRotation(direction);

        if (Time.time >= lastFireTime + fireRate)
        {
            Shoot();
            lastFireTime = Time.time;
        }
    }

    void PatrolToNewPoint()
    {
        Vector3 randomDir = Random.insideUnitSphere * patrolRadius;
        randomDir += startPosition;
        if (NavMesh.SamplePosition(randomDir, out NavMeshHit hit, patrolRadius, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }
    }

    void Shoot()
    {
        if (bulletPrefab == null || firePoint == null || player == null) return;

        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.LookRotation(player.transform.position - firePoint.position));
        var rb = bullet.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = (player.transform.position - firePoint.position).normalized * 20f;
        }

        var bulletScript = bullet.GetComponent<UniversalBullet>();
        if (bulletScript != null)
        {
            bulletScript.shooterTeam = myTeam.team;
        }

        Debug.Log($"{name} shot at {player.name}");
    }
}
