using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Sirenix.OdinInspector;

[Serializable]
public class ControllersHub
{
    [SerializeField]
    [Required]
    GameObject aiControllerPrefab;

    Dictionary<Team, List<Controller>> controllerLists = new Dictionary<Team, List<Controller>>();

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

        // TODO: Rework logic
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
