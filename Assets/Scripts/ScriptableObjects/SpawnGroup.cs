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

    public Texture2D SpawnIcon => spawnIcon;
}

[Serializable]
public struct UnitCountInfo
{
    public GameObject prefab;

    public int count;
}
