using UnityEngine;

public class EnemyAnimatorController : MonoBehaviour
{
    private Animator animator;

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
    }
    public void SetMoving(bool isMoving)
    {
        animator.SetBool("isMoving", isMoving);
    }
    public void SetPatrolling(bool isPatrolling)
    {
        animator.SetBool("isPatrolling", isPatrolling);
    }

    public void PlayShoot()
    {
        animator.SetTrigger("isShooting");
    }

    public void PlayHit()
    {
        animator.SetTrigger("isHit");
    }

    public void PlayDeath()
    {
        animator.SetBool("isDead", true);
    }
}
