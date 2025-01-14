using UnityEngine;
using Unity.Netcode;
public class MachineGun : NetworkBehaviour
{
    public GameObject Projectile;
    public Transform Camera;
    public float Speed = 5000;
    public ulong ID;
    public NetworkObject networkObject;
    public float Delay;
    float nextTimeToFire = 0;
    public void Start()
    {
        SendSetIdRpc(networkObject.NetworkObjectId);
    }

    [Rpc(SendTo.Server)]
    public void SendSetIdRpc(ulong ID1)
    {
        SetIdRpc(ID1);
    }

    [Rpc(SendTo.ClientsAndHost)]
    public void SetIdRpc(ulong ID1)
    {
        ID = ID1;
    }

    void Update()
    {
        if (Input.GetButton("Fire1") && IsOwner)
        {
            if (nextTimeToFire < Time.time)
            {
                nextTimeToFire = Time.time + Delay;
                ShootSendRpc(Camera.rotation, Camera.transform.forward, ID);
            }
        }
    }

    [Rpc(SendTo.Server)]
    public void ShootSendRpc(Quaternion rotation, Vector3 ForwardVector, ulong PlayerID)
    {
        ShootRpc(rotation, ForwardVector, PlayerID);
    }

    [Rpc(SendTo.ClientsAndHost)]
    public void ShootRpc(Quaternion rotation, Vector3 ForwardVector, ulong PlayerID)
    {
        GameObject Fish = Instantiate(Projectile, Camera.position, rotation);
        Fish.transform.position = Camera.transform.position + ForwardVector;
        Fish.transform.rotation = rotation;
        Fish.GetComponent<Rigidbody>().AddForce(ForwardVector * Speed);
        Fish.GetComponent<Bullet>().ID = PlayerID;
    }
}
