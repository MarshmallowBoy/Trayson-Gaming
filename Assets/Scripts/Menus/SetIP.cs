using Unity.Netcode.Transports.UTP;
using UnityEngine;

public class SetIP : MonoBehaviour
{
    public UnityTransport UnityTransport;
    public void SetAddress(string Address)
    {
        UnityTransport.ConnectionData.Address = Address;
    }
}
