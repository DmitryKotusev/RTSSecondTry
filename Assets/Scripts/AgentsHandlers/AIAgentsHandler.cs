using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class AIAgentsHandler : MonoBehaviour, IAgentsHandler
{
    [SerializeField]
    [Required]
    private Controller controller;

    private List<Agent> availableAgents = new List<Agent>();

    public List<Agent> GetAllAvailableAgents()
    {
        return availableAgents;
    }

    public void RegisterAgent(Agent agent)
    {
        availableAgents.Add(agent);
        agent.SetController(controller);
    }

    public void RegisterAgents(IEnumerable<Agent> newAgents)
    {
        availableAgents.AddRange(newAgents);

        foreach (Agent agent in newAgents)
        {
            agent.SetController(controller);
        }
    }

    public void UnregisterAgent(Agent agent)
    {
        availableAgents.Remove(agent);
        agent.SetController(null);
    }
}
