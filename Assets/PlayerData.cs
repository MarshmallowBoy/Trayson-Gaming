using Steamworks;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerData : NetworkBehaviour
{
    public string Name = "%Username%";
    public TextMeshPro text1;
    void Update()
    {
        if (SteamManager.Initialized)
        {
            Name = SteamFriends.GetPersonaName();
            SetStringAcrossNetworkRPC(Name);
        }
    }

    [Rpc(SendTo.Server)]
    public void SetStringAcrossNetworkRPC(string str)
    {
        RecieveStringAcrossNetworkRPC(str);
    }

    [Rpc(SendTo.ClientsAndHost)]
    public void RecieveStringAcrossNetworkRPC(string str)
    {
        Name = str;
        text1.text = str; 
    }
}
