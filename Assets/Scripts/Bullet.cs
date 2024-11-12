using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int Damage = 10;
    public bool Collision = false;
    public ulong ID;
    public bool Damaged = false;
    public float Speed = 1400;

    private void Start()
    {
        GetComponent<Rigidbody>().AddForce(transform.forward * Speed);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !Damaged)
        {
            if (other.GetComponent<FishThrower>().ID == ID)
            {
                return;
            }
            other.gameObject.GetComponent<Heath>().health -= Damage;
            Damaged = true;
            Destroy(gameObject);
        }
        if(!other.CompareTag("Player") && !other.CompareTag("Bullet"))
        {
            Damage = 0;
        }
    }
}
