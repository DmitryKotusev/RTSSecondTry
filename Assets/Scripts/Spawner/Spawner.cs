using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public abstract class Spawner : MonoBehaviour
{
    [SerializeField]
    [Required]
    protected BattlePointsManager battlePointsManager;

    [SerializeField]
    protected SpawnPoint spawnPoint;

    [SerializeField]
    protected MonoBehaviour agentsHandler;

    [SerializeField]
    [Required]
    protected AvailableGroupsInfo availableGroupsInfo;

    public virtual bool SpawnGroup(SpawnGroup spawnGroup)
    {
        if (spawnPoint == null)
        {
            return false;
        }

        if (battlePointsManager.CurrentBattlePointsAmount < spawnGroup.PointsCost)
        {
            return false;
        }

        if (spawnPoint.SpawnGroup(spawnGroup))
        {
            battlePointsManager.CurrentBattlePointsAmount -= spawnGroup.PointsCost;

            return true;
        }

        return false;
    }

    public IAgentsHandler AgentsHandler => agentsHandler as IAgentsHandler;

    public AvailableGroupsInfo AvailableGroupsInfo => availableGroupsInfo;
}
