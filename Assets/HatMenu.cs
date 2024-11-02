using UnityEngine;
using Unity.Netcode;
using Unity.Networking;
using NUnit.Framework;
public class HatMenu : NetworkBehaviour
{
    public GameObject[] Hat;

    public void Addhat(int Index)
    {
        AddHatSendRpc(Index);
    }


    [Rpc(SendTo.Server)]
    public void AddHatSendRpc(int Index)
    {
        AddHatRpc(Index);
    }

    [Rpc(SendTo.ClientsAndHost)]
    void AddHatRpc(int Index)
    {
        foreach (var h in Hat)
        {
            h.SetActive(false);
        }
        Hat[Index].SetActive(true);
    }

    void Start()
    {
        gameObject.SetActive(false);
    }
}
