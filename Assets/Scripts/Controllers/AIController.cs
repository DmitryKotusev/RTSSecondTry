using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class AIController : Controller
{
    [SerializeField]
    [Required]
    AIAgentsHandler agentsHandler;

    public override IAgentsHandler GetAgentsHandler()
    {
        return agentsHandler;
    }
}
