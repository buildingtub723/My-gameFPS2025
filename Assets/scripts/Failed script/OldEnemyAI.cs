using UnityEngine;
using UnityEngine.AI;
using System.Collections;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(TeamIdentity))]
public class OldEnemyAI : MonoBehaviour
{
    public enum State { Patrolling, Chasing, StrafingAndShooting, Hunting };
    [SerializeField] private LayerMask visibilityMask = ~0; // Set to "Everything" by default

    public float patrolRadius = 10f;
    public float detectionRange = 20f;
    public float shootingRange = 15f;
    public float fireRate = 1.5f;
    public float huntingDuration = 10f;
    public bool startInHuntingMode = false;
    public GameObject bulletPrefab;
    public Transform firePoint;

    public bool isInvestigating = false;
    private Vector3 investigationPoint;
    private float investigationTime = 0f;
    private float maxInvestigationDuration = 5f;


    private NavMeshAgent agent;
    private TeamIdentity myTeam;
    private GameObject currentTarget;
    private State currentState = State.Patrolling;
    private float lastFireTime = 0f;
    private Vector3 startPosition;
    private float huntingTimer = 0f;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        myTeam = GetComponent<TeamIdentity>();
        startPosition = transform.position;

        if (startInHuntingMode)
            currentState = State.Hunting;
        else
            PatrolToNewPoint();
    }

    void Update()
    {
        FindTarget();

        //  Add this logic:
        if (currentTarget != null)
        {
            float distanceToTarget = Vector3.Distance(transform.position, currentTarget.transform.position);

            if (distanceToTarget <= shootingRange)
            {
                currentState = State.StrafingAndShooting;
            }
            else
            {
                currentState = State.Chasing;
            }
        }
        else if (currentState != State.Hunting && !isInvestigating)
        {
            currentState = State.Patrolling;
        }

        // Now process the current state
        switch (currentState)
        {
            case State.Patrolling:
                HandlePatrolling();
                break;
            case State.Chasing:
                HandleChasing();
                break;
            case State.StrafingAndShooting:
                HandleStrafingAndShooting();
                break;
            case State.Hunting:
                HandleHunting();
                break;
        }
        if (currentTarget != null)
        {
            Debug.Log($"{name} sees target: {currentTarget.name}");
        }
    }
    public void StartInvestigation(Vector3 point)
    {
        Debug.Log($"{name} starting investigation at {point}");
        investigationPoint = point;
        isInvestigating = true;
        investigationTime = 0f;
        currentState = State.Hunting;
    }
    void HandlePatrolling()
    {
        if (!agent.pathPending && agent.remainingDistance < 1f)
        {
            PatrolToNewPoint();
        }

        if (currentTarget != null)
        {
            currentState = State.Chasing;
        }
    }

    void HandleChasing()
    {
        Debug.Log($"{name} is Chasing {currentTarget?.name}");
        if (currentTarget == null)
        {
            currentState = State.Hunting;
            huntingTimer = huntingDuration;
            return;
        }

        float dist = Vector3.Distance(transform.position, currentTarget.transform.position);

        if (dist <= shootingRange && dist > 5f)
        {
            currentState = State.StrafingAndShooting;
            return;
        }

        agent.isStopped = false;
        agent.SetDestination(currentTarget.transform.position);
    }

    void HandleStrafingAndShooting()
    {
        Debug.Log($"{name} is Shooting at {currentTarget?.name}");
        if (currentTarget == null)
        {
            currentState = State.Hunting;
            huntingTimer = huntingDuration;
            return;
        }

        float dist = Vector3.Distance(transform.position, currentTarget.transform.position);
        if (dist > shootingRange)
        {
            currentState = State.Chasing;
            return;
        }

        Vector3 directionToTarget = (currentTarget.transform.position - transform.position).normalized;
        Vector3 strafeDir = Vector3.Cross(Vector3.up, directionToTarget).normalized;

        float strafeDirection = Mathf.Sin(Time.time * 2f);
        Vector3 strafeTarget = transform.position + strafeDir * strafeDirection;

        agent.isStopped = false;
        agent.SetDestination(strafeTarget);

        Vector3 lookDir = directionToTarget;
        lookDir.y = 0;
        if (lookDir != Vector3.zero)
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookDir), Time.deltaTime * 5f);

        if (Time.time >= lastFireTime + fireRate)
        {
            Shoot();
            lastFireTime = Time.time;
        }
    }

    void HandleHunting()
    {
        Debug.Log($"{name} is Hunting");
        if (currentTarget != null)
        {
            currentState = State.Chasing;
            return;
        }

        agent.isStopped = false;
        agent.SetDestination(investigationPoint);

        float distance = Vector3.Distance(transform.position, investigationPoint);
        if (distance < 1.5f)
        {
            investigationTime += Time.deltaTime;

            // Look around while waiting (optional rotation)
            transform.Rotate(Vector3.up * Time.deltaTime * 60f);

            if (investigationTime >= maxInvestigationDuration)
            {
                isInvestigating = false;
                currentState = State.Patrolling;
            }
        }
    }

    void Shoot()
    {

        Vector3 direction = (currentTarget.transform.position - firePoint.position).normalized;
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.LookRotation(direction));
        Debug.Log($"{name} fired a bullet toward {currentTarget.name}");

        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = direction * 20f; // Use .velocity, not .linearVelocity
        }
        var bulletScript = bullet.GetComponent<UniversalBullet>();
        if (bulletScript != null)
        {
            bulletScript.shooterTeam = myTeam.team;
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

    void FindTarget()
    {
        GameObject bestTarget = null;
        float closestDist = detectionRange;

        foreach (TeamIdentity other in Object.FindObjectsByType<TeamIdentity>(FindObjectsSortMode.None))
        {
            if (other == myTeam || other.team == myTeam.team) continue;

            float dist = Vector3.Distance(transform.position, other.transform.position);
            if (dist < closestDist)
            {
                Vector3 dirToTarget = (other.transform.position - transform.position).normalized;
                Vector3 origin = transform.position + Vector3.up * 1.5f;

                // Raycast to ensure no walls block vision
                if (Physics.Raycast(origin, dirToTarget, out RaycastHit hit, detectionRange))
                {
                    Debug.DrawRay(origin, dirToTarget * detectionRange, Color.red, 0.1f);

                    if (hit.collider.GetComponentInParent<TeamIdentity>() == other)
                    {
                        bestTarget = other.gameObject;
                        closestDist = dist;
                        Debug.Log($"[Target Acquired] {other.name}");
                    }
                }
            }
        }

        currentTarget = bestTarget;

        if (currentTarget != null)
        {
            Debug.Log($"{name} sees target: {currentTarget.name}");
        }
    }
}
