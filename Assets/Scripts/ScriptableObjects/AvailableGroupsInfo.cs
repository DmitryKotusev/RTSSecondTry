using System.Linq;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "CustomScriptables/AvailableGroupsInfo")]
public class AvailableGroupsInfo : ScriptableObject
{
    [SerializeField]
    private List<SpawnGroup> availableSpawnGroups = new List<SpawnGroup>();

    public List<SpawnGroup> AvailableSpawnGroups => new List<SpawnGroup>(availableSpawnGroups);

    public SpawnGroup GetMostExpensiveGroup()
    {
        SpawnGroup mostExpensiveGroup = null;

        float maxCost = -1f;

        foreach (SpawnGroup spawnGroup in availableSpawnGroups)
        {
            if (spawnGroup.PointsCost > maxCost)
            {
                mostExpensiveGroup = spawnGroup;

                maxCost = spawnGroup.PointsCost;
            }
        }

        return mostExpensiveGroup;
    }

    public List<SpawnGroup> AvailableSpawnGroupsSortedByCost(bool inverse = true)
    {
        List<SpawnGroup> sortedGroups = new List<SpawnGroup>(availableSpawnGroups);

        sortedGroups.Sort((spawnGroup1, spawnGroup2) =>
        {
            if (inverse)
            {
                return spawnGroup2.PointsCost.CompareTo(spawnGroup1.PointsCost);
            }

            return spawnGroup1.PointsCost.CompareTo(spawnGroup2.PointsCost);
        });

        return sortedGroups;
    }
}
