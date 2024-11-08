using UnityEngine;
using Unity.Netcode;
public class FishThrower : NetworkBehaviour
{
    public GameObject Mackerel;
    public Transform Camera;
    public float PositionOffset;
    public float Speed = 5000;
    void Update()
    {
        if (Input.GetButtonDown("Fire1") && IsOwner)
        {
            ShootSendRpc();
        }
    }

    [Rpc(SendTo.Server)]
    public void ShootSendRpc()
    {
        ShootRpc();
    }

    [Rpc(SendTo.ClientsAndHost)]
    public void ShootRpc()
    {
        GameObject Fish = Instantiate(Mackerel, Camera.position, Camera.rotation);
        Fish.transform.position = Camera.transform.position + Camera.transform.forward * PositionOffset;
        Fish.transform.rotation = Camera.transform.rotation;
        Fish.GetComponent<Rigidbody>().AddForce(Camera.forward * Speed);
    }
}
