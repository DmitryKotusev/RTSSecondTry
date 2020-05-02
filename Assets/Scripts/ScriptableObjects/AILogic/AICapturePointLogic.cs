using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(menuName = "CustomScriptables/AICapturePointLogic")]
public class AICapturePointLogic : AILogic
{
    [SerializeField]
    private float checkSpawnPosibilityPeriod = 10f;

    private List<CapturePoint> capturePoints = new List<CapturePoint>();

    private AIBattlePointsManager aIBattlePointsManager;

    private AISpawner aISpawner;

    private AIAgentsHandler aIAgentsHandler;

    private float timeSinceLastCheck = 0f;

    public override void Init(AIController aIController)
    {
        this.aIController = aIController;

        aIBattlePointsManager = aIController?.GetComponent<AIBattlePointsManager>();

        aISpawner = aIController?.GetComponent<AISpawner>();

        aIAgentsHandler = aIController?.GetComponent<AIAgentsHandler>();

        FindCapturePoints();
    }

    public override void Update()
    {
        TrySpawnNewBattleGroup();

        // Possible logic extention
    }

    private void FindCapturePoints()
    {
        capturePoints.Clear();
        capturePoints.AddRange(FindObjectsOfType<CapturePoint>());
    }

    private void TrySpawnNewBattleGroup()
    {
        timeSinceLastCheck += Time.deltaTime;

        if (timeSinceLastCheck < checkSpawnPosibilityPeriod)
        {
            return;
        }

        timeSinceLastCheck = 0;

        float currentBattlePoints = aIBattlePointsManager.CurrentBattlePointsAmount;

        float currentCommandPoints = aIBattlePointsManager.CurrentCommandPointsAmount;

        float commandPointsLimit = aIBattlePointsManager.PointsInfo.CommandPointsLimit;

        List<SpawnGroup> sortedSpawnGroups = aISpawner.AvailableGroupsInfo.AvailableSpawnGroupsSortedByCost();

        int spawnedAgentsCount = 0;

        foreach (SpawnGroup spawnGroup in sortedSpawnGroups)
        {
            float groupCommandPointsCost = spawnGroup.CommandPointsCost;

            float groupBattlePointsCost = spawnGroup.PointsCost;

            if (currentCommandPoints + groupCommandPointsCost >= commandPointsLimit)
            {
                continue;
            }

            if (!aISpawner.SpawnGroup(spawnGroup))
            {
                break;
            }

            spawnedAgentsCount = spawnGroup.AgentsCount;

            break;
        }

        if (spawnedAgentsCount == 0)
        {
            return;
        }

        GiveCommandToSpawnedUnits(aIAgentsHandler.GetAllAvailableAgents().GetRange(aIAgentsHandler.GetAllAvailableAgents().Count - spawnedAgentsCount, spawnedAgentsCount));
    }

    private void GiveCommandToSpawnedUnits(List<Agent> spawnedAgents)
    {
        // uniform distribution

        for (int i = 0; i < spawnedAgents.Count; i++)
        {
            int capturePointToGoIndex =  i % capturePoints.Count;

            CapturePoint capturePointToGo = capturePoints[capturePointToGoIndex];

            Agent agentToSend = spawnedAgents[i];

            SendAgentToCapturePoint(capturePointToGo, agentToSend);
        }
    }

    private void SendAgentToCapturePoint(CapturePoint capturePoint, Agent agent)
    {
        float randomRotation = UnityEngine.Random.Range(0, 360);

        Vector3 randomDirection = Quaternion.AngleAxis(randomRotation, Vector3.up) * new Vector3(1, 0, 0);

        Vector3 agentDestiantion = capturePoint.transform.position + randomDirection * UnityEngine.Random.Range(0, 1.2f * capturePoint.CapturePointData.CaptureRadius);

        agent.SetNewGoal(new MoveGoal(agentDestiantion));
    }
}
