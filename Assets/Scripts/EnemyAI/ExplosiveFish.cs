using Unity.VisualScripting;
using UnityEngine;

public class ExplosiveFish : MonoBehaviour
{
    public GameObject Explosion;
    public float ExplosionRadius;
    public float force;
    public Vector3 DirectionBias;
    public float ExplosionDamage;
    private void OnDestroy()
    {
        Instantiate(Explosion, transform.position, Quaternion.identity);
        foreach(var Coll in Physics.OverlapSphere(transform.position, ExplosionRadius))
        {
            if (Coll.CompareTag("Player"))
            {
                Coll.gameObject.GetComponent<SC_FPSController>().ExternalVector += (((Coll.transform.position - transform.position).normalized + DirectionBias) * force) / (((Vector3.Distance(Coll.transform.position, transform.position)+1))/ExplosionRadius);
                if(Coll.transform.GetComponent<FishThrower>().ID == GetComponent<Bullet>().ID)
                {
                    return;
                }
                Coll.transform.GetComponent<Heath>().health -= Mathf.RoundToInt(ExplosionDamage / ((Vector3.Distance(Coll.transform.position, transform.position) + 5)/ExplosionRadius));
            }
        }
        
    }
}
