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
    }

    private void GiveMovementCommandToFormation()
    {
        // TODO: remake
        Formation currentFormation = selectionManager.GetCurrentFormation();

        Agent formationLeader = currentFormation.GetFormationLeader();

        formationLeader.SetNewGoal(commandsManager.CurrentGoalToCommand);
    }
}
