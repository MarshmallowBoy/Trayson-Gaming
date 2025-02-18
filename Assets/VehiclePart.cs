using Unity.Netcode;
using UnityEngine;
using UnityEngine.Audio;

public class VehiclePart : MonoBehaviour
{
    public int health = 500;

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
        health = health1;
    }
}
