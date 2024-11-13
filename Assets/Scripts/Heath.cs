using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
public class Heath : NetworkBehaviour
{
    public int health = 100;
    int healthLastFrame = 100;
    public Slider healthSlider;
    public CharacterController characterController;
    public Animator animator;
    public AudioSource audioSource;
    private void Update()
    {
        healthSlider.value = health;

        /*
        if (health < healthLastFrame)
        {
            animator.Play("Damaged");
            SendUpdateHealthRpc(health);
        }*/

        if (health <= 0)
        {
            characterController.enabled = false;
            transform.position = Vector3.zero;
            characterController.enabled = true;
            health = 100;
        }
        healthLastFrame = health;
    }

    public void DoDamage(int Damage)
    {
        SendUpdateHealthRpc(health - Damage);
    }

    [Rpc(SendTo.Server)]
    void SendUpdateHealthRpc(int health1)
    {
        ReceiveUpdateHealthRpc(health1);
    }

    [Rpc(SendTo.ClientsAndHost)]
    void ReceiveUpdateHealthRpc(int health1)
    {
        audioSource.Play(); 
        animator.Play("Damaged");
        health = health1;
    }
}
