using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class AIController : Controller
{
    [SerializeField]
    [Required]
    private AILogic aiLogicSample;

    [SerializeField]
    [Required]
    private AIAgentsHandler agentsHandler;

    private AILogic aILogic;

    public override IAgentsHandler GetAgentsHandler()
    {
        return agentsHandler;
    }

    public override void Awake()
    {
        base.Awake();

        aILogic = Instantiate(aiLogicSample);

        aILogic?.Init(this);
    }

    private void Update()
    {
        aILogic.Update();
    }
}
