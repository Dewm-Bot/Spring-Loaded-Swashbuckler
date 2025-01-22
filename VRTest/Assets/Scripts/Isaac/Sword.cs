using UnityEngine;

public class Sword : MonoBehaviour
{
    [SerializeField] private float swordDamage;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.gameObject.name + " Trigger");

        if (other.gameObject.TryGetComponent<Enemy>(out Enemy enemy))
            enemy.ChangeEnemyHealth(-10);
    }

    public void SwordActive()
    {
        GetComponent<BoxCollider>().isTrigger = true;
    }
    public void SwordInactive()
    {
        GetComponent<BoxCollider>().isTrigger = false;
    }
}
