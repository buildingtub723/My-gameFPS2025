using UnityEngine;
using System.Collections;
using NodeCanvas.BehaviourTrees;

public class NpcSoldierDeathHandler : MonoBehaviour, IDeathHandler
{
    public float deathDelay = 5f;
    private NpcSoldierAudioManager audioManager;
    private BehaviourTreeOwner behaviourTree;
    private UnityEngine.AI.NavMeshAgent agent;
    private Collider[] colliders;
    private Health health;
    private bool isDead = false;

    private void Awake()
    {
        audioManager = GetComponent<NpcSoldierAudioManager>();
        behaviourTree = GetComponent<BehaviourTreeOwner>();
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        colliders = GetComponentsInChildren<Collider>();
        health = GetComponent<Health>();
    }

    public void HandleDeath(GameObject npc)
    {
        if (isDead) return;
        isDead = true;

        //  Step 1: play the death sound first before disabling anything
        if (audioManager != null)
        {
            audioManager.PlayDeathSound();
        }

        //  Step 2: stop all logic & movement
        if (behaviourTree != null)
            behaviourTree.enabled = false;

        if (agent != null)
            agent.enabled = false;

        //  Step 3: disable colliders (optional: keep ragdoll if you use one)
        foreach (var col in colliders)
            col.enabled = false;

        //  Step 4: disable other scripts like AI behaviour, shooting, etc.
        MonoBehaviour[] scripts = GetComponents<MonoBehaviour>();
        foreach (var s in scripts)
        {
            if (s != this && s != audioManager) // keep audio manager alive
                s.enabled = false;
        }

        //  Step 5: schedule destruction
        StartCoroutine(DelayedDestroy());
    }

    private IEnumerator DelayedDestroy()
    {
        yield return new WaitForSeconds(deathDelay);
        Destroy(gameObject);
    }
}