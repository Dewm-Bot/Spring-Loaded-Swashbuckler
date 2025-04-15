using TMPro;
using UnityEngine;
using UnityEngine.AI;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class EnemyAgent : Agent
{

    private NavMeshAgent navAgent;
    private Vector3 lastPosition; 

    [Header("Health Settings")]
    [SerializeField] private float enemyHealth = 100f;
    [SerializeField] private TMP_Text enemyHealthText;

    [Header("Respawn Settings")]
    public EnemyManager enemyManager;

    [Header("Spawn Settings")]
    [Tooltip("Assign a spawn point for step reset")]
    public Transform spawnPoint; //Debug for ML

    [Header("Combat Settings")]
    public Transform player;
    public float attackRange = 2.0f;
    public int attackDamage = 10;
    private float attackCooldown = 1.0f;
    private float lastAttackTime;

    //Reward params
    private float previousDistanceToPlayer;

    [Header("Separation Settings")]
    public float minimumSeparationDistance = 1.5f;
    public LayerMask enemyLayer;

    [Header("Attack Range Optimization")]
    public float attackRangeWindow = 0.5f;

    [Header("Target Direction Reward")]
    [Tooltip("Weight for moving in the desired target direction.")]
    public float targetDirRewardMultiplier = 0.02f;
    public float targetDirOffset = 0f; //player tracking offset
    private float stagnationThreshold = 2f; 
    
    
    public override void Initialize()
    {
        navAgent = GetComponent<NavMeshAgent>();
        UpdateHealthText();
        lastPosition = transform.position;
        previousDistanceToPlayer = HorizontalDistance(transform.position, player.position);
    }
    
    //Sensors
    public override void CollectObservations(VectorSensor sensor)
    {
        Vector3 relativePlayerPosition = player.position - transform.position;
        sensor.AddObservation(relativePlayerPosition);
        sensor.AddObservation(navAgent.velocity);
        sensor.AddObservation(transform.forward);
        sensor.AddObservation(Time.time >= lastAttackTime + attackCooldown);
    }

    //Primary loop
    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        HandleMovement(actionBuffers);
        HandleAttack();
        UpdateRewardShaping();
    }


private void HandleMovement(ActionBuffers actionBuffers)
{
    float moveX = actionBuffers.ContinuousActions[0];
    float moveZ = actionBuffers.ContinuousActions[1];
    Vector3 inputDir = new Vector3(moveX, 0, moveZ).normalized;

    if (inputDir.sqrMagnitude > 0.01f)
    {
        Vector3 proposedDestination = transform.position + inputDir * navAgent.speed;
        NavMeshPath path = new NavMeshPath();
        if (navAgent.CalculatePath(proposedDestination, path) && path.status == NavMeshPathStatus.PathComplete)
        {
            navAgent.SetDestination(proposedDestination);
        }
    }
        lastPosition = transform.position;
    }

    private void HandleAttack()
    {
        if (Time.time - lastAttackTime >= attackCooldown &&
            Vector3.Distance(transform.position, player.position) < attackRange)
        {
            AttackPlayer();
            lastAttackTime = Time.time;
        }
    }

    private void AttackPlayer()
    {
        AddReward(2.0f);  // Reward for a successful attack.
        if (player.TryGetComponent<PlayerHealth>(out PlayerHealth human))
        {
            human.TakeDamage(attackDamage);
        }
    }
    
    
    //Reward shaping nightmare. Don't ask.
    private void UpdateRewardShaping()
    {
        //Reward for reducing horizontal distance to the player.
        float currentDistance = HorizontalDistance(transform.position, player.position);
        float distanceDelta = previousDistanceToPlayer - currentDistance;
        AddReward(distanceDelta * 0.05f);
        previousDistanceToPlayer = currentDistance;

        //Reward for continuous movement (penalize lack of movement).
        float speed = navAgent.velocity.magnitude;
        if (speed < 0.1f)
            AddReward(-0.005f);
        else
            AddReward(0.001f);

        //Extra reward for heading in the right direction.
        Vector3 movementDir = navAgent.velocity.normalized;
        Vector3 desiredDir = (player.position - transform.position).normalized;
        desiredDir = Quaternion.Euler(0, targetDirOffset, 0) * desiredDir;
        float alignment = Vector3.Dot(movementDir, desiredDir);
        AddReward(targetDirRewardMultiplier * alignment);

        //Separation from other enemies.
        Collider[] enemyColliders = Physics.OverlapSphere(transform.position, minimumSeparationDistance, enemyLayer);
        foreach (Collider col in enemyColliders)
        {
            if (col.gameObject != gameObject)
            {
                float separationDist = Vector3.Distance(transform.position, col.transform.position);
                if (separationDist < minimumSeparationDistance)
                    AddReward(-0.02f * Mathf.Pow((minimumSeparationDistance - separationDist), 2));
            }
        }

        //Reward for staying within an optimal attack range.
        float playerDistance = Vector3.Distance(transform.position, player.position);
        if (playerDistance <= attackRange)
            AddReward(0.05f);
        else
            AddReward(-0.01f * (playerDistance - attackRange));
    }
    
    private float HorizontalDistance(Vector3 a, Vector3 b)
    {
        return Vector3.Distance(new Vector3(a.x, 0, a.z), new Vector3(b.x, 0, b.z));
    }
    
    public void ChangeEnemyHealth(float healthChange)
    {
        enemyHealth += healthChange;
        UpdateHealthText();
        if (enemyHealth <= 0)
            Die();
    }

    private void UpdateHealthText()
    {
        if (enemyHealthText != null)
            enemyHealthText.text = $"{enemyHealth}";
    }

    private void Die()
    {
        if (enemyManager != null)
            enemyManager.RespawnEnemy(gameObject);
        else
            Destroy(gameObject);
        enemyHealth = 100f;
        UpdateHealthText();
    }

    public override void OnEpisodeBegin()
    {
        //Reset position and rotation using the assigned spawn point.
        if (spawnPoint != null)
        {
            navAgent.Warp(spawnPoint.position);
            transform.rotation = spawnPoint.rotation;
        }
        else
        {
            navAgent.ResetPath();
        }
        enemyHealth = 100f;
        UpdateHealthText();
        previousDistanceToPlayer = HorizontalDistance(transform.position, player.position);
    }
    
    // Heuristic for Testing
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActionsOut = actionsOut.ContinuousActions;
        continuousActionsOut[0] = Random.Range(-1f, 1f);
        continuousActionsOut[1] = Random.Range(-1f, 1f);
    }
}
