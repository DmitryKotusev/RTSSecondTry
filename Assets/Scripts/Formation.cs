using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Formation : IEnumerable<Agent>
{
    [Tooltip("List of formation agents, leader NOT included!")]
    [SerializeField]
    List<Agent> formationAgents = new List<Agent>();

    [SerializeField]
    Agent formationLeader;

    public Formation()
    {
        formationLeader = null;
    }

    public Formation(Agent formationLeader, IEnumerable<Agent> formationAgents = null)
    {
        if (formationAgents != null)
        {
            foreach (Agent agent in formationAgents)
            {
                AddAgentToFormation(agent);
            }
        }

        RemoveAgentFromFormation(formationLeader);

        this.formationLeader = formationLeader;
    }

    public bool DoesBelongToFormation(Agent agent)
    {
        if (IsLeader(agent))
        {
            return true;
        }

        return formationAgents.Any((formationAgent) =>
        {
            return formationAgent == agent;
        });
    }

    public bool IsEmpty()
    {
        return !(formationLeader != null || formationAgents.Count > 0);
    }

    public bool IsLeader(Agent agent)
    {
        return agent == formationLeader;
    }

    public Agent GetFormationLeader()
    {
        return formationLeader;
    }

    public void SetFormationLeader(Agent formationLeader)
    {
        Agent oldLeader = this.formationLeader;
        this.formationLeader = formationLeader;
        formationAgents.Remove(formationLeader);
        formationAgents.Add(oldLeader);
    }

    public List<Agent> GetFormationAgentsWithoutLeader()
    {
        return new List<Agent>(formationAgents);
    }

    public List<Agent> GetAllFormationAgents()
    {
        List<Agent> allAgentsList = new List<Agent>(formationAgents);
        if (formationLeader != null)
        {
            allAgentsList.Add(formationLeader);
        }
        return allAgentsList;
    }

    public int GetAllFormationAgentsCount()
    {
        List<Agent> allAgentsList = new List<Agent>(formationAgents);
        allAgentsList.Add(formationLeader);
        return GetAllFormationAgents().Count;
    }

    public bool AddAgentToFormation(Agent formationAgent)
    {
        if (formationAgent == formationLeader)
        {
            return false;
        }

        if (formationLeader == null)
        {
            formationLeader = formationAgent;
            return true;
        }

        if (formationAgents.Contains(formationAgent))
        {
            return false;
        }
        formationAgents.Add(formationAgent);

        return true;
    }

    public bool RemoveAgentFromFormation(Agent formationAgent)
    {
        if (formationAgent == formationLeader)
        {
            if (formationAgents.Count >= 1)
            {
                formationLeader = formationAgents[formationAgents.Count - 1];
                return formationAgents.Remove(formationLeader);
            }
            else
            {
                formationLeader = null;
                return true;
            }
        }
        return formationAgents.Remove(formationAgent);
    }

    public override bool Equals(object obj)
    {
        //Check for null and compare run-time types.
        if ((obj == null) || !this.GetType().Equals(obj.GetType()))
        {
            return false;
        }

        Formation otherFormation = obj as Formation;

        if (formationLeader != otherFormation.formationLeader)
        {
            return false;
        }

        foreach(Agent agent in formationAgents)
        {
            if (!otherFormation.formationAgents.Contains(agent))
            {
                return false;
            }
        }

        return true;
    }

    public IEnumerator<Agent> GetEnumerator()
    {
        return GetAllFormationAgents().GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
