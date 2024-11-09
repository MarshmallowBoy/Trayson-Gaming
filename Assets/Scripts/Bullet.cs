using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int Damage = 10;
    public bool Collision = false;
    public bool HasDoneDamage = false;
    private void Start()
    {
        Invoke("EnableDamage", 0.02f);
    }

    void EnableDamage()
    {
        Collision = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && Collision && !HasDoneDamage)
        {
            other.gameObject.GetComponent<Heath>().health -= Damage;
            HasDoneDamage = true;
            Destroy(gameObject);
        }
        if(!other.CompareTag("Player") && !other.CompareTag("Bullet"))
        {
            Damage = 0;
        }
    }
}
