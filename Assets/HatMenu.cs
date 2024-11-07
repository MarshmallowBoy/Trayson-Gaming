using UnityEngine;
using Unity.Netcode;
public class HatMenu : NetworkBehaviour
{
    public GameObject[] Hat;

    public int Activehat = -1;

    public void Addhat(int Index)
    {
        Activehat = Index;
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



    public void PlayerConnected(ulong Player)
    {
        if(Activehat < 0)
        {
            return;
        }
        Addhat(Activehat);
    }

    void Start()
    {
        gameObject.SetActive(false);
        NetworkManager.Singleton.OnClientConnectedCallback += PlayerConnected;
    }
}
