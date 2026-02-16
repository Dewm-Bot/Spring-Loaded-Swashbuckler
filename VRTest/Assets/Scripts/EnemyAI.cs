using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public Animator animator;
    public Transform target; // XR Origin or Player Transform
    public float detectionRange = 10f;
    public float attackRange = 2f;
    public float moveSpeed = 2f;

    private int damageCount = 0;
    private bool isDead = false;
    private NavMeshAgent agent;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (target == null)
        {
            GameObject xrOrigin = GameObject.FindWithTag("Player"); // Tag XR Origin or main player
            if (xrOrigin != null)
                target = xrOrigin.transform;
        }

        agent = GetComponent<NavMeshAgent>();
        if (agent != null)
            agent.speed = moveSpeed;

        if (animator == null)
            animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isDead || target == null) return;

        float distance = Vector3.Distance(transform.position, target.position);

        if (distance < detectionRange)
        {
            animator.SetTrigger("Notice");

            if (distance <= attackRange)
            {
                agent.ResetPath();
                animator.SetBool("isWalking", false);

                int rand = Random.Range(0, 2);
                if (rand == 0)
                    animator.SetTrigger("Attack1");
                else
                    animator.SetTrigger("Attack2");
            }
            else
            {
                agent.SetDestination(target.position);
                animator.SetBool("isWalking", true);
            }
        }
        else
        {
            animator.SetBool("isWalking", false);
            agent.ResetPath();
            animator.SetTrigger("Idle");
        }
    }
    void OnMouseDown()
    {
        if (isDead) return;

        damageCount++;
        animator.SetTrigger("Damaged");

        if (damageCount >= 10)
        {
            Die();
        }
    }

    void Die()
    {
        isDead = true;
        animator.SetTrigger("Death");
        agent.isStopped = true;
        // Optionally destroy the enemy after death animation
        Destroy(gameObject, 3f);
    }
}
