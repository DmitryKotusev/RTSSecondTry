using UnityEngine;
using Sirenix.OdinInspector;
using System.Collections.Generic;

public class LevelManager : MonoBehaviour
{
    [SerializeField]
    [Required]
    TeamsVisibleUnitsStore unitsStore;

    [SerializeField]
    [Required]
    ControllersHub controllersHub;

    static public LevelManager Instance { get; private set; }

    public TeamsVisibleUnitsStore UnitsStore
    {
        get
        {
            return unitsStore;
        }
    }

    private void Awake()
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

    private void Update()
    {
        UpdateUnitsInFieldOfViewOfEachTeam();
    }

    private void UpdateUnitsInFieldOfViewOfEachTeam()
    {
        unitsStore.ClearTeamsVisibleUnits();

        FindUnitsInFieldOfViewOfEachTeam();
    }

    private void FindUnitsInFieldOfViewOfEachTeam()
    {
        foreach(KeyValuePair<Team, List<Controller>> controllerList in controllersHub.ControllerLists)
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
