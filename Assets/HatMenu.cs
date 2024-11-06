using UnityEngine;
using Unity.Netcode;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using Unity.Networking;
public class HatMenu : NetworkBehaviour
{
    public GameObject[] Hat;

    public int Activehat = -1;

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


    public void OnPlayerConnected()
    {
        Debug.Log("Player Joined");
    }

    void Start()
    {
        gameObject.SetActive(false);
    }
}
