using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System;

public class PlayerController : Controller
{
    [SerializeField]
    [Required]
    Camera playersCamera;

    [SerializeField]
    [Required]
    SelectionManager selectionManager;

    [SerializeField]
    [Required]
    CommandsManager commandsManager;

    private void Update()
    {
        SendCurrentCommandsToCurrentFormation();
    }

    void SendCurrentCommandsToCurrentFormation()
    {
        if (selectionManager.GetCurrentFormation() == null)
        {
            return;
        }
        commandsManager.CheckCommands();

        if (commandsManager.CurrentGoalToCommand == null)
        {
            return;
        }

        if (commandsManager.CurrentGoalToCommand is MoveGoal)
        {
            GiveMovementCommandToFormation();
        }

        if (commandsManager.CurrentGoalToCommand is AttackGoal)
        {
            GiveAttackCommandToFormation();
        }
    }

    private void GiveMovementCommandToFormation()
    {
        // TODO: remake
        Formation currentFormation = selectionManager.GetCurrentFormation();

        Agent formationLeader = currentFormation.GetFormationLeader();

        List<Agent> agents = currentFormation.GetFormationAgentsWithoutLeader();

        List<Vector3> agentsDestinations = GetAgentsDestinations(
            (commandsManager.CurrentGoalToCommand as MoveGoal).Destination, agents.Count, formationLeader.AgentRadius);

        for (int i = 0; i < agents.Count; i++)
        {
            agents[i].SetNewGoal(new MoveGoal(agentsDestinations[i]));
        }

        formationLeader.SetNewGoal(commandsManager.CurrentGoalToCommand);
    }

    private List<Vector3> GetAgentsDestinations(Vector3 leadersDestination, int agentsCount, float agentsRadius)
    {
        List<Vector3> resultList = new List<Vector3>(agentsCount);

        int agentsDestinationsLeftToCount = agentsCount;

        if (agentsDestinationsLeftToCount <= 0)
        {
            return resultList;
        }

        float baseUnitOffset = agentsRadius * LevelManager.Instance.RadiusMultiplier;

        for (int i = 1; ; i++)
        {
            float cycleFormationRadius = baseUnitOffset * i;
            int currentCycleAgentsCount = i * 8;
            int subListAgentsCount = i * 2;

            for (int j = 0; j < currentCycleAgentsCount; j++)
            {
                int quotient = j / 4;
                int leftover = j % 4;

                float x = 0;
                float z = 0;

                switch (leftover)
                {
                    // 1
                    case 0:
                        {
                            if (quotient % 2 == 0)
                            {
                                x = leadersDestination.x - cycleFormationRadius + baseUnitOffset * (quotient / 2);
                            }
                            else
                            {
                                x = leadersDestination.x - cycleFormationRadius + baseUnitOffset * (subListAgentsCount - 1 - (quotient / 2));
                            }
                            z = leadersDestination.z + cycleFormationRadius;

                            break;
                        }
                    // 2
                    case 1:
                        {
                            if (quotient % 2 == 0)
                            {
                                x = leadersDestination.x + cycleFormationRadius - baseUnitOffset * (quotient / 2);
                            }
                            else
                            {
                                x = leadersDestination.x + cycleFormationRadius - baseUnitOffset * (subListAgentsCount - 1 - (quotient / 2));
                            }
                            z = leadersDestination.z - cycleFormationRadius;

                            break;
                        }
                    // 3
                    case 2:
                        {
                            x = leadersDestination.x + cycleFormationRadius;
                            if (quotient % 2 == 0)
                            {
                                z = leadersDestination.z + cycleFormationRadius - baseUnitOffset * (quotient / 2);
                            }
                            else
                            {
                                z = leadersDestination.z + cycleFormationRadius - baseUnitOffset * (subListAgentsCount - 1 - (quotient / 2));
                            }

                            break;
                        }
                    // 4
                    case 3:
                        {
                            x = leadersDestination.x - cycleFormationRadius;
                            if (quotient % 2 == 0)
                            {
                                z = leadersDestination.z - cycleFormationRadius + baseUnitOffset * (quotient / 2);
                            }
                            else
                            {
                                z = leadersDestination.z - cycleFormationRadius + baseUnitOffset * (subListAgentsCount - 1 - (quotient / 2));
                            }

                            break;
                        }
                }

                Vector3 newDestination = new Vector3(x, leadersDestination.y, z);
                resultList.Add(newDestination);
                agentsDestinationsLeftToCount--;

                if (agentsDestinationsLeftToCount == 0)
                {
                    break;
                }
            }

            if (agentsDestinationsLeftToCount == 0)
            {
                break;
            }
        }

        return resultList;
    }

    private void GiveAttackCommandToFormation()
    {
        // TODO: remake
        Formation currentFormation = selectionManager.GetCurrentFormation();

        Agent formationLeader = currentFormation.GetFormationLeader();

        List<Agent> agents = currentFormation.GetFormationAgentsWithoutLeader();

        List<Vector3> agentsDestinations = GetAgentsDestinations(
            (commandsManager.CurrentGoalToCommand as AttackGoal).Destination, agents.Count, formationLeader.AgentRadius);

        for (int i = 0; i < agents.Count; i++)
        {
            agents[i].SetNewGoal(new AttackGoal(
                (commandsManager.CurrentGoalToCommand as AttackGoal).AgentToAttack,
                agentsDestinations[i]
                ));
        }

        formationLeader.SetNewGoal(commandsManager.CurrentGoalToCommand);
    }
}
