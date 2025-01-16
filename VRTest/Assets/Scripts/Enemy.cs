using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private float enemyHealth;

    private void Update()
    {
        if (enemyHealth <= 0)
        {
            Destroy(gameObject);
        }
    }

    public void ChangeEnemyHealth(float health)
    {
        enemyHealth += health;
        Debug.Log(enemyHealth);
    }
}
