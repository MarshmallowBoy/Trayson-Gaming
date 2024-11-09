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
            ShootSendRpc(Camera.rotation, Camera.transform.forward);
        }
    }

    [Rpc(SendTo.Server)]
    public void ShootSendRpc(Quaternion rotation, Vector3 ForwardVector)
    {
        ShootRpc(rotation, ForwardVector);
    }

    [Rpc(SendTo.ClientsAndHost)]
    public void ShootRpc(Quaternion rotation, Vector3 ForwardVector)
    {
        GameObject Fish = Instantiate(Mackerel, Camera.position, rotation);
        Fish.transform.position = Camera.transform.position + ForwardVector * PositionOffset;
        Fish.transform.rotation = rotation;
        Fish.GetComponent<Rigidbody>().AddForce(ForwardVector * Speed);
    }
}
