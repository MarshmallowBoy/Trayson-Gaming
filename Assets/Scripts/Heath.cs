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

        if (health < healthLastFrame)
        {
            animator.Play("Damaged");
            SendSoundRpc();
        }

        if (health <= 0)
        {
            characterController.enabled = false;
            transform.position = Vector3.zero;
            characterController.enabled = true;
            health = 100;
        }
        healthLastFrame = health;
    }

    [Rpc(SendTo.Server)]
    void SendSoundRpc()
    {
        ReceiveSoundRpc();
    }

    [Rpc(SendTo.ClientsAndHost)]
    void ReceiveSoundRpc()
    {
        audioSource.Play();
    }
}
