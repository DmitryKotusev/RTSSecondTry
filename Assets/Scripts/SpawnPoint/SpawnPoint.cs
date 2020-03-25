using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    [SerializeField]
    private Controller controller;

    [SerializeField]
    private LayerMask environmentLayerMask;

    [SerializeField]
    private SpawnPointInfo spawnPointInfo;

    public Controller Controller
    {
        get => controller;
        set => controller = value;
    }

    public bool SpawnGroup(SpawnGroup spawnGroup)
    {
        List<GameObject> objectsToSpawn = new List<GameObject>();

        foreach (UnitCountInfo unitCountInfo in spawnGroup.SpawnPairs)
        {
            int unitsCount = unitCountInfo.count;

            if (unitsCount < 1)
            {
                continue;
            }

            objectsToSpawn.Add(unitCountInfo.prefab);
        }

        int squareRoot = (int)Mathf.Sqrt(objectsToSpawn.Count);

        Vector3 startSpawnPoint = FindSpawnPoint();

        if (startSpawnPoint == Vector3.positiveInfinity)
        {
            return false;
        }

        for (int index = 0; index < objectsToSpawn.Count; index++)
        {
            int i = index / squareRoot;

            int j = index % squareRoot;
        }
    }

    private Vector3 FindSpawnPoint()
    {
        Vector3 currentPoint = transform.position;

        for (int i = 0; i < spawnPointInfo.TryFindStartTries; i++)
        {
            Vector3 groundPoint = GetGroundPoint(currentPoint);

            if (groundPoint == Vector3.positiveInfinity)
            {
                continue;
            }

            // Check other units presence
        }

        return Vector3.positiveInfinity;
    }

    private Vector3 GetGroundPoint(Vector3 point)
    {
        RaycastHit raycastHit;

        if (Physics.Raycast(point,
            Vector3.up,
            out raycastHit,
            spawnPointInfo.SpawnHeightCheckDistance,
            environmentLayerMask))
        {
            return raycastHit.point;
        }
        else if (Physics.Raycast(
            point,
            -Vector3.up,
            out raycastHit,
            spawnPointInfo.SpawnHeightCheckDistance,
            environmentLayerMask))
        {
            return raycastHit.point;
        }

        return Vector3.positiveInfinity;
    }
}
