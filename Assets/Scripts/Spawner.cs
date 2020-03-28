using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class Spawner : MonoBehaviour
{
    [SerializeField]
    [Required]
    private BattlePointsManager battlePointsManager;

    [SerializeField]
    private SpawnPoint spawnPoint;

    [SerializeField]
    private MonoBehaviour agentsHandler;

    public bool SpawnGroup(SpawnGroup spawnGroup)
    {
        return true;
    }

    public IAgentsHandler AgentsHandler => agentsHandler as IAgentsHandler;
}
