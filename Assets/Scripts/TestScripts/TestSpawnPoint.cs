using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class TestSpawnPoint : MonoBehaviour
{
    [SerializeField]
    private SpawnPoint spawnPoint;

    [SerializeField]
    private SpawnGroup spawnGroup;

    [Button]
    public void Spawn()
    {
        spawnPoint.SpawnGroup(spawnGroup);
    }
}
