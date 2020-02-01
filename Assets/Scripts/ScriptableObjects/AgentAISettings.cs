using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "Assets/Prefabs/Scriptables/AgentAISettings/AgentAISettings", menuName = "CustomScriptables/AgentAISettings")]
public class AgentAISettings : ScriptableObject
{
    [BoxGroup("Attack settings")]
    [SerializeField]
    float checkForCloseEnemyInAttackPeriod = 2f;

    [BoxGroup("Idle behaivor settings")]
    [SerializeField]
    private float checkForCloseEnemyInIdlePeriod = 0.5f;

    [BoxGroup("Idle behaivor settings")]
    [SerializeField]
    private float checkForCloseEnemyInMovePeriod = 0.5f;

    [BoxGroup("Move behaivor settings")]
    [SerializeField]
    [Tooltip("Seconds of check end of agents path cycle when agents are moving")]
    private float agentsSecondsTillCheckEndPath = 1f;

    public float CheckForCloseEnemyInAttackPeriod
    {
        get
        {
            return checkForCloseEnemyInAttackPeriod;
        }
    }

    public float CheckForCloseEnemyInIdlePeriod
    {
        get
        {
            return checkForCloseEnemyInIdlePeriod;
        }
    }

    public float CheckForCloseEnemyInMovePeriod
    {
        get
        {
            return checkForCloseEnemyInMovePeriod;
        }
    }

    public float AgentsSecondsTillCheckEndPath
    {
        get
        {
            return agentsSecondsTillCheckEndPath;
        }
    }
}
