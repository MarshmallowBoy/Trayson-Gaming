using TND.XeSS;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;
public class ConsoleCommands : MonoBehaviour
{
    public string CurrentParameter;
    public XeSS_URP XeSS_URP;
    public GameObject[] treetypes;
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
        SceneManager.LoadScene(int.Parse(CurrentParameter));
    }

}
