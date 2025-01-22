using TMPro;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private float enemyHealth;
    [SerializeField] private TMP_Text enemyHealthText;

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
        enemyHealthText.text = $"{enemyHealth}";
        Debug.Log(enemyHealth);
    }
}
