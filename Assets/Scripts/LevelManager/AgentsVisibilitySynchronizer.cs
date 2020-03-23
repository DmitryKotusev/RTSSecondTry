using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentsVisibilitySynchronizer
{
    public void UpdateUnitsInFieldOfViewOfEachTeam(TeamsVisibleUnitsStore unitsStore, ControllersHub controllersHub)
    {
        unitsStore.ClearTeamsVisibleUnits();

        FindUnitsInFieldOfViewOfEachTeam(unitsStore, controllersHub);
    }

    public void FindUnitsInFieldOfViewOfEachTeam(TeamsVisibleUnitsStore unitsStore, ControllersHub controllersHub)
    {
        foreach (KeyValuePair<Team, List<Controller>> controllerList in controllersHub.ControllerLists)
        {
            Team team = controllerList.Key;
            List<Controller> oneTeamControllers = controllerList.Value;

            foreach (Controller controller in oneTeamControllers)
            {
                IAgentsHandler agentsHandler = controller.GetAgentsHandler();

                List<Agent> agents = agentsHandler.GetAllAvailableAgents();

                foreach (Agent agent in agents)
                {
                    EyeSightManager eyeSightManager = agent.GetEyeSightManager();

                    List<Unit> unitsInFieldOfView = eyeSightManager.GetEnemyUnitsInFieldOfView(agent.GetSettings().LookDistance, agent.GetTeam());

                    unitsStore.RegisterVisibleUnitsRange(agent.GetTeam(), unitsInFieldOfView);
                }
            }
        }
    }
}
