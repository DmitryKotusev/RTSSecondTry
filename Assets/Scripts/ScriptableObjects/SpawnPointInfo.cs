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
    private float spawnHeightObstacleDistance = 5f;

    [SerializeField]
    private int tryFindStartTries = 1000;

    [SerializeField]
    private List<Vector3> spawnPointOffsets = new List<Vector3>();

    public float DefaultSpawnDistance => defaultSpawnDistance;

    public float SpawnHeightCheckDistance => spawnHeightCheckDistance;

    public float SpawnHeightObstacleDistance => spawnHeightObstacleDistance;

    public int TryFindStartTries => tryFindStartTries;

    public List<Vector3> SpawnPointOffsets => new List<Vector3>(spawnPointOffsets);
}
