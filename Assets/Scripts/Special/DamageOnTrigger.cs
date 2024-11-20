using UnityEngine;

public class DamageOnTrigger : MonoBehaviour
{
    public int Damage;
    private void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("Player"))
        {
            other.transform.GetComponent<Heath>().DoDamage(Damage);
        }
    }
}
