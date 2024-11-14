using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int Damage = 10;
    public bool Collision = false;
    public ulong ID;
    public bool Damaged = false;
    public bool DestroyOnTouch;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !Damaged)
        {
            if (other.GetComponent<FishThrower>().ID == ID)
            {
                return;
            }
            //other.gameObject.GetComponent<Heath>().health -= Damage;
            other.gameObject.GetComponent<Heath>().DoDamage(Damage);
            Damaged = true;
            Destroy(gameObject);
        }
        if(!other.CompareTag("Player") && !other.CompareTag("Bullet"))
        {
            Damage = 0;
            if (DestroyOnTouch)
            {
                Destroy(gameObject);
            }
        }
    }
}
