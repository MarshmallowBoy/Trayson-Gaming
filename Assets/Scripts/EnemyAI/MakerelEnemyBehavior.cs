using UnityEngine;
using UnityEngine.AI;

public class MakerelEnemyBehavior : MonoBehaviour
{
    public float FindRadius;
    public Transform Target;
    public float distance;
    public NavMeshAgent agent;
    public int Damage;
    private void Update()
    {
        FindTarget();
        if (Target != null)
        {
            agent.SetDestination(Target.position);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.gameObject.GetComponent<Heath>().health -= Damage;
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
