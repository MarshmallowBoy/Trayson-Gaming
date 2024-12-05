using Unity.Netcode;
using UnityEngine;
using UnityEngine.ProBuilder.Shapes;
using UnityEngine.Rendering.VirtualTexturing;

public class Bullet : MonoBehaviour
{
    public int Damage = 10;
    public bool Collision = false;
    public ulong ID;
    public bool Damaged = false;
    public bool DestroyOnTouch;
    public ParticleSystem[] NonDestroyParticles;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !Damaged)
        {
            if (other.GetComponent<NetworkObject>().NetworkObjectId == ID)
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
                if (NonDestroyParticles.Length > 0)
                {
                    foreach (var particle in NonDestroyParticles)
                    {
                        var em = particle.emission;
                        em.enabled = false;
                        particle.transform.parent = null;
                        particle.transform.localScale = Vector3.one;
                    }
                }
                Destroy(gameObject);
            }
        }
    }
}
