using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "CustomScriptables/AvailableGroupsInfo")]
public class AvailableGroupsInfo : ScriptableObject
{
    [SerializeField]
    private List<SpawnGroup> availableSpawnGroups = new List<SpawnGroup>();

    public List<SpawnGroup> AvailableSpawnGroups => new List<SpawnGroup>(availableSpawnGroups);
}
