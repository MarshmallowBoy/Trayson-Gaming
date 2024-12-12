using TND.XeSS;
using UnityEngine;
using System;
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



        /*
            Terrain terrain = GameObject.Find("TerrainMain").GetComponent<Terrain>();
            TerrainData terrainData = terrain.terrainData;
            foreach (var proto in terrainData.treePrototypes)
            {
                proto.prefab = treetypes[int.Parse(CurrentParameter)];
            }
            terrain.Flush();
        */
    }


}
