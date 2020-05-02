using System.Collections.Generic;
using UnityEngine;
using System;
using Sirenix.OdinInspector;

[CreateAssetMenu(menuName = "CustomScriptables/SpawnGroup")]
public class SpawnGroup : ScriptableObject
{
    [SerializeField]
    private List<UnitCountInfo> spawnPairs;

    [SerializeField]
    private float pointsCost = 100f;

    [SerializeField]
    [Required]
    private Texture2D spawnIcon;

    public List<UnitCountInfo> SpawnPairs => spawnPairs;

    public float PointsCost => pointsCost;

    public float CommandPointsCost
    {
        get
        {
            float totalWeight = 0;

            foreach (UnitCountInfo spawnPair in spawnPairs)
            {
                GameObject prefab = spawnPair.prefab;

                Agent agent = prefab.GetComponent<Agent>();

                if (agent != null)
                {
                    totalWeight += agent.GetSettings().AgentSpawnWeight * spawnPair.count;
                }
            }

            return totalWeight;
        }
    }

    public int AgentsCount
    {
        get
        {
            int agentsCount = 0;

            foreach (UnitCountInfo spawnPair in spawnPairs)
            {
                agentsCount += spawnPair.count;
            }

            return agentsCount;
        }
    }

    public Texture2D SpawnIcon => spawnIcon;
}

[Serializable]
public struct UnitCountInfo
{
    public GameObject prefab;

    public int count;
}
