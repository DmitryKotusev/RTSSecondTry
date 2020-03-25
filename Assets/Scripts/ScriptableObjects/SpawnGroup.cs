using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(menuName = "CustomScriptables/SpawnGroup")]
public class SpawnGroup : ScriptableObject
{
    [SerializeField]
    private List<UnitCountInfo> spawnPairs;

    public List<UnitCountInfo> SpawnPairs => spawnPairs;
}

[Serializable]
public struct UnitCountInfo
{
    public GameObject prefab;

    public int count;
}
