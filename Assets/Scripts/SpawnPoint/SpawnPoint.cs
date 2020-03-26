using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SpawnPoint : MonoBehaviour
{
    [SerializeField]
    private Controller controller;

    [SerializeField]
    private LayerMask environmentLayerMask;

    [SerializeField]
    private LayerMask obstacleLayerMask;

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

        List<Vector3> spawnPoints = new List<Vector3>();

        if (!FindSpawnPoints(objectsToSpawn, spawnPoints))
        {
            return false;
        }

        // Spawn objects on their points

        return true;
    }

    private bool FindSpawnPoints(
        List<GameObject> objectsToSpawn,
        List<Vector3> spawnPoints
        )
    {
        int squareRoot = (int)Mathf.Sqrt(objectsToSpawn.Count);

        Vector3 currentVerticalSpawnPoint = transform.position;

        for (int index = 0; index < objectsToSpawn.Count; index += squareRoot)
        {
            int subIndex = index;

            int i = subIndex / squareRoot;

            float maxRadius = FindMaxRadius(objectsToSpawn, i, 0, squareRoot);

            for (int tryIndex = 0; tryIndex <= spawnPointInfo.TryFindStartTries; tryIndex++)
            {
                if (tryIndex == spawnPointInfo.TryFindStartTries)
                {
                    return false;
                }

                currentVerticalSpawnPoint -= transform.forward * maxRadius;

                Vector3 groundPoint = GetGroundPoint(currentVerticalSpawnPoint);

                if (groundPoint == Vector3.positiveInfinity)
                {
                    continue;
                }

                if (IsObstaclePresent(groundPoint))
                {
                    continue;
                }

                spawnPoints.Add(groundPoint);

                break;
            }

            for (; subIndex < objectsToSpawn.Count && subIndex < index + squareRoot; subIndex++)
            {
                int j = subIndex % squareRoot;

                maxRadius = FindMaxRadius(objectsToSpawn, i, j, squareRoot);

                Vector3 currentHorizontalSpawnPoint = currentVerticalSpawnPoint;

                for (int tryIndex = 0; tryIndex <= spawnPointInfo.TryFindStartTries; tryIndex++)
                {
                    if (tryIndex == spawnPointInfo.TryFindStartTries)
                    {
                        return false;
                    }

                    currentHorizontalSpawnPoint += transform.right * maxRadius;

                    Vector3 groundPoint = GetGroundPoint(currentHorizontalSpawnPoint);

                    if (groundPoint == Vector3.positiveInfinity)
                    {
                        continue;
                    }

                    if (IsObstaclePresent(groundPoint))
                    {
                        continue;
                    }

                    spawnPoints.Add(groundPoint);

                    break;
                }
            }
        }

        return true;
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

    private bool IsObstaclePresent(Vector3 point)
    {
        Vector3 point1 = point + Vector3.up * spawnPointInfo.DefaultSpawnDistance;

        Vector3 point2 = point1 + spawnPointInfo.SpawnHeightObstacleDistance * Vector3.up;

        return Physics.CapsuleCast(
           point1,
           point2,
           spawnPointInfo.DefaultSpawnDistance,
           Vector3.up,
           0,
           obstacleLayerMask);
    }

    private bool IsObstaclePresent(Vector3 point, float spawnDistance)
    {
        Vector3 point1 = point + Vector3.up * spawnDistance;

        Vector3 point2 = point1 + spawnPointInfo.SpawnHeightObstacleDistance * Vector3.up;

        return Physics.CapsuleCast(
           point1,
           point2,
           spawnDistance,
           Vector3.up,
           0,
           obstacleLayerMask);
    }

    private float FindMaxRadius(List<GameObject> objectsToSpawn, int i, int j, int squareRoot)
    {
        float leftRadius = GetUnitSpawnRadius(objectsToSpawn, i, j - 1, squareRoot);

        float upRadius = GetUnitSpawnRadius(objectsToSpawn, i - 1, j, squareRoot);

        float centerRadius = GetUnitSpawnRadius(objectsToSpawn, i, j - 1, squareRoot);

        float maxRadius = Mathf.Max(leftRadius, upRadius, centerRadius);

        return maxRadius;
    }

    private float GetUnitSpawnRadius(List<GameObject> objectsToSpawn, int i, int j, int squareRoot)
    {
        if (i < 0 || i >= objectsToSpawn.Count)
        {
            return 0;
        }

        if (j < 0 || j >= objectsToSpawn.Count)
        {
            return 0;
        }

        int index = i * squareRoot + j;

        Agent agent = objectsToSpawn[index].GetComponent<Agent>();

        float radius = agent == null ? 0 : agent.GetSettings().SpawnDistance;

        return radius;
    }
}
