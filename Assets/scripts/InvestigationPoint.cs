using UnityEngine;

public class InvestigationPoint : MonoBehaviour
{
    public float detectionRadius = 15f;
    public float duration = 10f;

    void Start()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, detectionRadius);
        Debug.Log($"Investigation Point spawned at {transform.position}, detected {hits.Length} colliders");

        foreach (var hit in hits)
        {
            EnemyAI enemy = hit.GetComponent<EnemyAI>();
            if (enemy != null)
            {
                Debug.Log($"Enemy {enemy.name} will investigate.");
                //enemy.StartInvestigation(transform.position);
            }
        }

        Destroy(gameObject, duration);
    }

    // Optional debug gizmo
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}