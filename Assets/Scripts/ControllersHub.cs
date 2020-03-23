using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "Assets/Prefabs/Scriptables/Controllers/ControllersHub", menuName = "CustomScriptables/ControllersHub")]
public class ControllersHub : ScriptableObject
{
    [SerializeField]
    [Required]
    GameObject aiControllerPrefab;

    Dictionary<Team, List<Controller>> controllerLists = new Dictionary<Team, List<Controller>>();

    public Dictionary<Team, List<Controller>> ControllerLists
    {
        get
        {
            return controllerLists;
        }
    }

    public void GetAllUnits(List<Unit> units)
    {
        units.Clear();

        foreach (KeyValuePair<Team, List<Controller>> controllerList in controllerLists)
        {
            Team team = controllerList.Key;
            List<Controller> oneTeamControllers = controllerList.Value;

            foreach (Controller controller in oneTeamControllers)
            {
                IAgentsHandler agentsHandler = controller.GetAgentsHandler();

                List<Agent> agents = agentsHandler.GetAllAvailableAgents();

                foreach (Agent agent in agents)
                {
                    units.Add(agent.SoldierBasic);
                }
            }
        }
    }

    public void RegisterController(Controller controller)
    {
        Team controllersTeam = controller.GetTeam();

        if (controllerLists.ContainsKey(controllersTeam))
        {
            if (controllerLists[controllersTeam] == null)
            {
                controllerLists[controllersTeam] = new List<Controller>();
            }

            if (!controllerLists[controllersTeam].Contains(controller))
            {
                controllerLists[controllersTeam].Add(controller);
            }

            return;
        }

        controllerLists.Add(controllersTeam, new List<Controller>() { controller });
    }

    public void UnregisterController(Controller controller)
    {
        Team controllersTeam = controller.GetTeam();

        if (controllerLists.ContainsKey(controllersTeam))
        {
            controllerLists[controllersTeam].Remove(controller);
        }
    }

    public void FindControllerForAgent(Agent agent, Team desiredTeam = null)
    {
        Team agentsTeam = desiredTeam != null ? desiredTeam : agent.GetTeam();

        Controller appropriateController = FindAppropriateController(agent, agentsTeam);

        agent.SetController(appropriateController);

        RegisterNewAgent(agent);
    }

    private void RegisterNewAgent(Agent agent)
    {
        Controller agentsController = agent.GetController();

        IAgentsHandler agentsHandler = agentsController.GetAgentsHandler();

        // TODO: Rework logic for AI

        if (agentsHandler != null)
        {
            agentsHandler.RegisterAgent(agent);
        }
    }

    private Controller FindAppropriateController(Agent agent, Team agentsTeam)
    {
        if (!controllerLists.ContainsKey(agentsTeam) || controllerLists[agentsTeam].Count == 0)
        {
            Controller aiController = SpawnAIController();
            aiController.SetTeam(agentsTeam);
            RegisterController(aiController);

            return aiController;
        }

        // TODO: Rework logic for AI
        return controllerLists[agentsTeam][0];
    }

    private Controller SpawnAIController()
    {
        GameObject aiController = GameObject.Instantiate(aiControllerPrefab, Vector3.zero, Quaternion.identity);
        Controller controller = aiController.GetComponent<Controller>();
        Debug.Log("Spawned new AI controller");
        return controller;
    }
}
