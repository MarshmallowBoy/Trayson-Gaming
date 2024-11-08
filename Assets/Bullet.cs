using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int Damage = 10;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Destroy(gameObject);
        }
        else
        {
            Damage = 0;
        }
    }
}
