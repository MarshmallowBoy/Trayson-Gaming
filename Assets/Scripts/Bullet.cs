using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int Damage = 10;
    public bool Collision = false;
    public ulong ID;

    void EnableDamage()
    {
        Collision = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && Collision)
        {
            if (other.GetComponent<FishThrower>().ID == ID)
            {
                return;
            }
            other.gameObject.GetComponent<Heath>().health -= Damage;
            Destroy(gameObject);
        }
        if(!other.CompareTag("Player") && !other.CompareTag("Bullet"))
        {
            Damage = 0;
        }
    }
}
