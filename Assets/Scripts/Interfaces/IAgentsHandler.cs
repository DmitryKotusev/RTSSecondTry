using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAgentsHandler
{
    List<Agent> GetAllAvailableAgents();

    void RegisterAgent(Agent newAgent);

    void RegisterAgents(IEnumerable<Agent> newAgents);

    void UnregisterAgent(Agent oldAgent);
}
