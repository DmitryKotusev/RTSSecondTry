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
