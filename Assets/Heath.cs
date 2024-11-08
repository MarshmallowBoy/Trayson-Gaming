using UnityEngine;

public class Heath : MonoBehaviour
{
    public int health = 100;
    public CharacterController characterController;
    private void Update()
    {
        if (health < 0)
        {
            characterController.enabled = false;
            transform.position = Vector3.zero;
            characterController.enabled = true;
            health = 100;
        }
    }
}
