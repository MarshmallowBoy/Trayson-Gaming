using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations;
using static UnityEngine.GraphicsBuffer;

public class EyeballfishThrower : MonoBehaviour
{
    public GameObject FishPrefab;
    public Transform Target;
    public float distance;
    public float FindRadius;
    public float Speed = 1400;

    public float Delay = 0.1f;
    float nextTimeToFire = 0;

    private void Update()
    {
        FindTarget();
        if (Target == null) { return; }
        transform.LookAt(Target.position + (Vector3.up*2));
        if (Vector3.Distance(transform.position, Target.position) > FindRadius)
        {
            Target = null;
        }
        if (nextTimeToFire < Time.time)
        {
            nextTimeToFire = Time.time + Delay;
            GameObject Fish = Instantiate(FishPrefab, transform.position, transform.rotation);
            Fish.transform.position = transform.position + transform.forward;
            Fish.transform.rotation = transform.rotation;
            Fish.GetComponent<Rigidbody>().AddForce(transform.forward * Speed);
            Fish.GetComponent<Bullet>().ID = 999;
        }
    }

    void FindTarget()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, FindRadius);
        foreach (Collider collider in hitColliders)
        {
            if (collider.gameObject.CompareTag("Player"))
            {
                if (Vector3.Distance(collider.transform.position, transform.position) < distance)
                {
                    Target = collider.transform;
                    distance = Vector3.Distance(collider.transform.position, transform.position);
                }
                Debug.Log(collider.transform.name);
            }
        }
        distance = 999;
    }
}
