using Steamworks;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerData : NetworkBehaviour
{
    public string Name = "%Username%";
    public TextMeshPro text1;
    void Start()
    {
        NetworkManager.Singleton.OnClientConnectedCallback += InitializeCameraSelection;
        if (SteamManager.Initialized)
        {
            if (IsOwner)
            {
                Name = SteamFriends.GetPersonaName();
                SetStringAcrossNetworkRPC(Name, GetComponent<NetworkObject>().OwnerClientId);
            }
        }
    }

    public void InitializeCameraSelection(ulong PlayerID)
    {
        if (SteamManager.Initialized)
        {
            if (IsOwner)
            {
                Name = SteamFriends.GetPersonaName();
                SetStringAcrossNetworkRPC(Name, GetComponent<NetworkObject>().OwnerClientId);
            }
        }
    }

    [Rpc(SendTo.Server)]
    public void SetStringAcrossNetworkRPC(string str, ulong PlayerID)
    {
        RecieveStringAcrossNetworkRPC(str, PlayerID);
    }

    [Rpc(SendTo.ClientsAndHost)]
    public void RecieveStringAcrossNetworkRPC(string str, ulong PlayerID)
    {
       NetworkManager.Singleton.ConnectedClients[PlayerID].PlayerObject.GetComponent<PlayerData>().Name = str;
       NetworkManager.Singleton.ConnectedClients[PlayerID].PlayerObject.GetComponent<PlayerData>().text1.text = str;
    }
}
