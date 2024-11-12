using UnityEngine;
using Unity.Netcode;
public class FishThrower : NetworkBehaviour
{
    public GameObject Mackerel;
    public Transform Camera;
    public float PositionOffset;
    public float Speed = 5000;
    public ulong ID;

    public void Start()
    {
        SendSetIdRpc(NetworkManager.Singleton.LocalClientId);
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
        if (Input.GetButtonDown("Fire1") && IsOwner)
        {
            ShootSendRpc(Camera.rotation, Camera.transform.forward, ID);
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
        GameObject Fish = Instantiate(Mackerel, Camera.position, rotation);
        Fish.transform.position = Camera.transform.position + ForwardVector * PositionOffset;
        Fish.transform.rotation = rotation;
        Fish.GetComponent<Rigidbody>().AddForce(ForwardVector * Speed);
        Fish.GetComponent<Bullet>().ID = PlayerID;
    }
}
