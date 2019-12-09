using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class LevelManager : MonoBehaviour
{
    [BoxGroup("Common settings")]
    [SerializeField]
    [Tooltip("Seconds of check end of agents path cycle when agents are moving")]
    private float agentsSecondsTillCheckEndPath = 1f;
    public float AgentsSecondsTillCheckEndPath
    {
        get
        {
            return agentsSecondsTillCheckEndPath;
        }
    }

    [BoxGroup("Common settings")]
    [SerializeField]
    [Tooltip("Max amount of agents that can be selected per time")]
    private float maxAgentSelectionCount = 15f;
    public float MaxAgentSelectionCount
    {
        get
        {
            return maxAgentSelectionCount;
        }
    }

    [BoxGroup("Common settings")]
    [SerializeField]
    [Tooltip("Agent radius multiplier when counting formation agents position")]
    private float radiusMultiplier = 4f;
    public float RadiusMultiplier
    {
        get
        {
            return radiusMultiplier;
        }
    }

    static public LevelManager Instance { get; private set; }

    private void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
