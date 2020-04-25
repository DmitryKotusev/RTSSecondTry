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

            for (int i = 0; i < unitsCount; i++)
            {
                objectsToSpawn.Add(unitCountInfo.prefab);
            }
        }

        List<Vector3> spawnPoints = new List<Vector3>();

        if (!FindSpawnPoints(objectsToSpawn, spawnPoints))
        {
            return false;
        }

        SpawnObjects(objectsToSpawn, spawnPoints);

        return true;
    }

    private void SpawnObjects(List<GameObject> objectsToSpawn, List<Vector3> spawnPoints)
    {
        List<Agent> spawnedAgents = new List<Agent>();

        for (int i = 0; i < spawnPoints.Count; i++)
        {
            Agent newAgent = Instantiate(objectsToSpawn[i], spawnPoints[i], transform.rotation)
                .GetComponent<Agent>();

            if (newAgent != null)
            {
                spawnedAgents.Add(newAgent);

                if (controller != null)
                {
                    newAgent.ClearControllerInfo();
                }
                
            }
        }

        GiveAgentsToController(spawnedAgents);
    }

    private void GiveAgentsToController(List<Agent> spawnedAgents)
    {
        if (controller == null)
        {
            return;
        }

        if (spawnedAgents.Count == 0)
        {
            return;
        }

        IAgentsHandler agentsHandler = controller.GetAgentsHandler();

        agentsHandler.RegisterAgents(spawnedAgents);

        foreach (Agent agent in spawnedAgents)
        {
            agent.IncreaseControllersCommandPoints();
        }
    }

    private bool FindSpawnPoints(
        List<GameObject> objectsToSpawn,
        List<Vector3> spawnPoints
        )
    {
        int squareRoot = (int)Mathf.Sqrt(objectsToSpawn.Count);

        Vector3 currentVerticalSpawnPoint = GetStartPoint();

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

                Vector3 groundPoint = GetGroundPoint(currentVerticalSpawnPoint);

                if (!IsPointValid(groundPoint, maxRadius))
                {
                    currentVerticalSpawnPoint -= transform.forward * maxRadius;
                    continue;
                }

                break;
            }

            Vector3 currentHorizontalSpawnPoint = currentVerticalSpawnPoint;

            for (; subIndex < objectsToSpawn.Count && subIndex < index + squareRoot; subIndex++)
            {
                int j = subIndex % squareRoot;

                maxRadius = FindMaxRadius(objectsToSpawn, i, j, squareRoot);

                for (int tryIndex = 0; tryIndex <= spawnPointInfo.TryFindStartTries; tryIndex++)
                {
                    if (tryIndex == spawnPointInfo.TryFindStartTries)
                    {
                        return false;
                    }

                    Vector3 groundPoint = GetGroundPoint(currentHorizontalSpawnPoint);

                    if (!IsPointValid(groundPoint, maxRadius))
                    {
                        currentHorizontalSpawnPoint += transform.right * maxRadius;
                        continue;
                    }

                    spawnPoints.Add(groundPoint);

                    break;
                }

                currentHorizontalSpawnPoint += transform.right * maxRadius;
            }

            currentVerticalSpawnPoint -= transform.forward * maxRadius;
        }

        return true;
    }

    private Vector3 GetStartPoint()
    {
        Vector3 startPoint = transform.position;

        Vector3 groundPoint = GetGroundPoint(startPoint);

        if (!IsPointValid(groundPoint, spawnPointInfo.DefaultSpawnDistance))
        {
            foreach (Vector3 offset in spawnPointInfo.SpawnPointOffsets)
            {
                startPoint = transform.position + offset;

                groundPoint = GetGroundPoint(startPoint);

                if (IsPointValid(groundPoint, spawnPointInfo.DefaultSpawnDistance))
                {
                    break;
                }
            }
        }

        return startPoint;
    }

    private bool IsPointValid(Vector3 point, float radius)
    {
        return !(point.ToString() == Vector3.positiveInfinity.ToString()
            || IsObstaclePresent(point, radius));
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
        //Vector3 point1 = point + Vector3.up * spawnDistance;

        Vector3 point1 = point + Vector3.up * 0.01f;

        Vector3 point2 = point1 + spawnPointInfo.SpawnHeightObstacleDistance * Vector3.up;

        //return Physics.SphereCast(
        //   point1,
        //   spawnDistance,
        //   point2 - point1,
        //   out raycastHit,
        //   spawnPointInfo.SpawnHeightObstacleDistance,
        //   obstacleLayerMask);

        return Physics.Raycast(point1, point2 - point1, spawnPointInfo.SpawnHeightObstacleDistance, obstacleLayerMask);
    }

    private float FindMaxRadius(List<GameObject> objectsToSpawn, int i, int j, int squareRoot)
    {
        float leftRadius = GetUnitSpawnRadius(objectsToSpawn, i, j - 1, squareRoot);

        float upRadius = GetUnitSpawnRadius(objectsToSpawn, i - 1, j, squareRoot);

        float centerRadius = GetUnitSpawnRadius(objectsToSpawn, i, j, squareRoot);

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

        float radius = agent == null ? spawnPointInfo.DefaultSpawnDistance : agent.GetSettings().SpawnDistance;

        return radius;
    }
}
