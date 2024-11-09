using UnityEngine;
using UnityEngine.UI;
public class Heath : MonoBehaviour
{
    public int health = 100;
    public Slider healthSlider;
    public CharacterController characterController;
    private void Update()
    {
        healthSlider.value = health;
        if (health <= 0)
        {
            characterController.enabled = false;
            transform.position = Vector3.zero;
            characterController.enabled = true;
            health = 100;
        }
    }
}
