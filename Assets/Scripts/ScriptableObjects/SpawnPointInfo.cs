using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "CustomScriptables/SpawnPointInfo")]
public class SpawnPointInfo : ScriptableObject
{
    [SerializeField]
    private float defaultSpawnDistance;

    [SerializeField]
    private float spawnHeightCheckDistance = 20f;

    [SerializeField]
    private int tryFindStartTries = 1000;

    public float DefaultSpawnDistance => defaultSpawnDistance;

    public float SpawnHeightCheckDistance => spawnHeightCheckDistance;

    public int TryFindStartTries => tryFindStartTries;
}
