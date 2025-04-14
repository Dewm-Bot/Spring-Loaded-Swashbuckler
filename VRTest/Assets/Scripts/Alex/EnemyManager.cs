using UnityEngine;
using UnityEngine.AI;

public class EnemyManager : MonoBehaviour //Simple respawn manager for enemies
{
    [Tooltip("List of enemy respawn point transforms in the scene.")]
    public Transform[] enemyRespawnPoints;
    
    public void RespawnEnemy(GameObject enemy)
    {
        if (enemyRespawnPoints.Length == 0) return;

        int randomIndex = Random.Range(0, enemyRespawnPoints.Length);
        Transform spawnPoint = enemyRespawnPoints[randomIndex];
        enemy.transform.position = spawnPoint.position;
        
        NavMeshAgent navAgent = enemy.GetComponent<NavMeshAgent>();
        if (!navAgent)
        {
            navAgent.ResetPath();
        }

    }
}