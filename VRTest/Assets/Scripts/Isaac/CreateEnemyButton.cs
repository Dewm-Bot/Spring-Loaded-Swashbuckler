using UnityEngine;

public class CreateEnemyButton : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private Vector3 enemySpawn;
    [SerializeField] private Quaternion enemyRotation;

    public void CreateEnemy()
    {
        if (GameObject.FindGameObjectWithTag("Enemy"))
        {
            return;
        }

        GameObject temp = Instantiate(enemyPrefab, enemySpawn, enemyRotation);
        temp.tag = "Enemy";
    }
}
