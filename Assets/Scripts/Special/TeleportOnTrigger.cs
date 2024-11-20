using UnityEngine;

public class TeleportOnTrigger : MonoBehaviour
{
    public Vector3 Coordinates;

    private void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("Player"))
        {
            other.GetComponent<CharacterController>().enabled = false;
            other.transform.position = Coordinates;
            other.GetComponent<CharacterController>().enabled = true;
        }
    }
}
