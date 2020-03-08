using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIAgentsHandler : MonoBehaviour, IAgentsHandler
{
    private List<Agent> availableAgents = new List<Agent>();

    public List<Agent> GetAllAvailableAgents()
    {
        return availableAgents;
    }

    public void RegisterAgent(Agent agent)
    {
        availableAgents.Add(agent);
    }

    public void UnregisterAgent(Agent agent)
    {
        availableAgents.Remove(agent);
    }
}
