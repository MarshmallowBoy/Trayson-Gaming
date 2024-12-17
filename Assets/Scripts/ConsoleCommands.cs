using TND.XeSS;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;
using Unity.Netcode;
using System.Net;
using Unity.Netcode.Transports.UTP;
public class ConsoleCommands : MonoBehaviour
{
    public string CurrentParameter;
    public XeSS_URP XeSS_URP;
    public GameObject[] treetypes;
    public GameObject Player;
    public void XESS()
    {
        XeSS_URP.SetQuality(int.Parse(CurrentParameter));
    }

    public void tree_shadows()
    {
        Terrain terrain = GameObject.Find("TerrainMain").GetComponent<Terrain>();
        TerrainData terrainData = terrain.terrainData;

        TreePrototype[] treePrototypes = terrainData.treePrototypes;

        for (int i = 0; i < treePrototypes.Length; i++)
        {
            treePrototypes[i].prefab = treetypes[int.Parse(CurrentParameter)];
        }

        terrainData.treePrototypes = treePrototypes;

        terrain.Flush();
    }

    public void map()
    {
        mapRpc(CurrentParameter);
    }

    [Rpc(SendTo.Server)]
    public void mapRpc(string param)
    {
        NetworkManager.Singleton.Shutdown();
        SceneManager.LoadScene(int.Parse(param));
    }

    public void connect()
    {
        NetworkManager.Singleton.gameObject.GetComponent<UnityTransport>().ConnectionData.Address = CurrentParameter;
        NetworkManager.Singleton.StartClient();
    }

    public void disconnect()
    {
        NetworkManager.Singleton.Shutdown();
        SceneManager.LoadScene(0);
    }

    public void kill()
    {
        if (Player != null)
        {
            Player.GetComponent<Heath>().health = 0;
        }
    }
}
