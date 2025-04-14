using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class PlayerHealth : MonoBehaviour //This script manages the player's health and respawn logic.
{
    public int maxHealth = 100;
    public int currentHealth;
    public float respawnDelay = 3f;
    //Reference to center of spawn area
    public Transform playAreaCenter;
    //Radius of spawn randomization logic
    public float spawnRadius = 10f;

    void Start()
    {
        currentHealth = maxHealth;
    }
    
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if(currentHealth <= 0)
        {
            StartCoroutine(Respawn());
        }
    }

    IEnumerator Respawn() //This is for debugging and ML Agent purposes... please replace
    {
        yield return new WaitForSeconds(respawnDelay);
        Vector3 newPosition = GetRandomNavMeshPosition();
        transform.position = newPosition;
        currentHealth = maxHealth;
    }

    Vector3 GetRandomNavMeshPosition()
    {
        //Choose a random point inside a sphere, then find the closest point on the NavMesh.
        Vector3 randomDirection = Random.insideUnitSphere * spawnRadius;
        randomDirection += playAreaCenter.position;
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDirection, out hit, spawnRadius, NavMesh.AllAreas))
        {
            return hit.position + (Vector3.up * 2);
        }
        //Fallback to the center if no position was found.
        return playAreaCenter.position;
    }
}