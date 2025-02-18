﻿using UnityEngine;
using Sirenix.OdinInspector;
using Pathfinding;
using System;

[SelectionBase]
public class Agent : MonoBehaviour
{
    [BoxGroup("Projectors")]
    [SerializeField]
    [Tooltip("Selection marker used to mark agents when selected")]
    [Required]
    private SelectionMarker selectionMarker;
    public SelectionMarker SelectionMarker
    {
        get
        {
            return selectionMarker;
        }
    }

    [BoxGroup("Settings")]
    [SerializeField]
    [Tooltip("Agent's settings")]
    [Required]
    private AgentSettings agentSettings;

    [SerializeField]
    [Tooltip("Controller that is able to give commands to this unit")]
    private Controller controller;

    [SerializeField]
    [Tooltip("Default team in which agent will try to find controller")]
    [Required]
    private Team team;

    [SerializeField]
    [Tooltip("Controllers' hub that is the place where an agent can find a controller")]
    [Required]
    private ControllersHub controllersHub;

    [SerializeField]
    [Tooltip("AI path handler")]
    [Required]
    private RichAI aiPathHandler;

    [SerializeField]
    [Required]
    private AgentWeaponManager weaponManager;

    [SerializeField]
    [Required]
    private EyeSightManager eyeSightManager;

    [SerializeField]
    [Required]
    private SoldierBasic soldierBasic;
    public SoldierBasic SoldierBasic
    {
        get
        {
            return soldierBasic;
        }
    }

    private Formation currentFormation = null;

    private Goal currentGoal = null;

    private State currentState = null;

    /////////////Idle behavior//////////
    [BoxGroup("Common AI settings")]
    [SerializeField]
    [Required]
    AgentAISettings agentAISettings;
    ////////////////////////////////////

    // Getters and setters
    #region
    public Vector3 GetVelocity()
    {
        return aiPathHandler.velocity;
    }

    public float CheckForCloseEnemyInAttackPeriod
    {
        get
        {
            return agentAISettings.CheckForCloseEnemyInAttackPeriod;
        }
    }

    public float CheckForCloseEnemyInIdlePeriod
    {
        get
        {
            return agentAISettings.CheckForCloseEnemyInIdlePeriod;
        }
    }

    public float CheckForCloseEnemyInMovePeriod
    {
        get
        {
            return agentAISettings.CheckForCloseEnemyInMovePeriod;
        }
    }

    public float AgentsSecondsTillCheckEndPath
    {
        get
        {
            return agentAISettings.AgentsSecondsTillCheckEndPath;
        }
    }

    public RichAI GetAIPathHandler()
    {
        return aiPathHandler;
    }

    public Team GetTeam()
    {
        if (controller != null)
        {
            return controller.GetTeam();
        }
        return team;
    }

    public EyeSightManager GetEyeSightManager()
    {
        return eyeSightManager;
    }

    public AgentWeaponManager GetWeaponManager()
    {
        return weaponManager;
    }

    public AgentSettings GetSettings()
    {
        return agentSettings;
    }

    public float AgentRadius
    {
        get
        {
            return aiPathHandler.radius;
        }
    }

    public void SetController(Controller controller)
    {
        this.controller = controller;
    }

    public Controller GetController()
    {
        return controller;
    }

    public void SetCurrentFormation(Formation currentFormation)
    {
        this.currentFormation = currentFormation;
    }

    public Formation GetCurrentFormation()
    {
        return currentFormation;
    }

    public void ClearCurrentFormation()
    {
        currentFormation = null;
    }

    public void SetNewGoal(Goal newGoal)
    {
        currentGoal = newGoal;

        if (newGoal is MoveGoal)
        {
            currentState?.Stop();
            currentState = new MoveState(
                this,
                (newGoal as MoveGoal).Destination,
                agentAISettings.CheckForCloseEnemyInMovePeriod
                );
            currentState.Start();
        }

        if (newGoal is AttackGoal)
        {
            ColliderCostPair targetColliderCostPair = CheckAttackGoalTargetAvailability(newGoal as AttackGoal);

            if (targetColliderCostPair != null)
            {
                if (!(currentState is AttackState))
                {
                    currentState?.Stop();
                    currentState = new AttackState(this, agentAISettings.CheckForCloseEnemyInAttackPeriod, targetColliderCostPair.collider.transform);
                    currentState.Start();
                }
            }
            else
            {
                currentState?.Stop();
                currentState = new MoveState(
                this,
                (newGoal as AttackGoal).Destination,
                agentAISettings.CheckForCloseEnemyInAttackPeriod
                );
                currentState.Start();
            }
        }
    }

    public ColliderCostPair CheckAttackGoalTargetAvailability(AttackGoal attackGoal)
    {
        if (!eyeSightManager.IsEnemyReachableAtDistance(attackGoal.AgentToAttack.SoldierBasic, agentSettings.LookDistance))
        {
            return null;
        }

        var enemyColliderCostPair = eyeSightManager.GetUnitsVisibleBodyPart(attackGoal.AgentToAttack.SoldierBasic);
        return enemyColliderCostPair;
    }

    public State CurrentState
    {
        get
        {
            return currentState;
        }
        set
        {
            currentState = value;
        }
    }

    public Goal GetCurrentGoal()
    {
        return currentGoal;
    }

    public void ClearCurrentGoal()
    {
        currentGoal = null;
    }
    #endregion

    public void FindRandomControllerForAgent()
    {
        if (controller != null)
        {
            return;
        }

        controllersHub.FindControllerForAgent(this);

        IncreaseControllersCommandPoints();
    }

    public void IncreaseControllersCommandPoints()
    {
        BattlePointsManager battlePointsManager = controller.BattlePointsManager;

        battlePointsManager.CurrentCommandPointsAmount += GetSettings().AgentSpawnWeight;
    }

    public void DecreaseControllersCommandPoints()
    {
        BattlePointsManager battlePointsManager = controller?.BattlePointsManager;

        if (battlePointsManager != null)
        {
            battlePointsManager.CurrentCommandPointsAmount -= GetSettings().AgentSpawnWeight;
        }
    }

    public void ClearControllerInfo()
    {
        IAgentsHandler agentsHandler = controller?.GetAgentsHandler();

        DecreaseControllersCommandPoints();

        agentsHandler?.UnregisterAgent(this);

        controller = null;
    }

    private void OnEnable()
    {
        FindRandomControllerForAgent();
    }

    private void OnDisable()
    {
        ClearControllerInfo();
    }
    
    private void Update()
    {
        CheckCurrentState();
        CurrentBehaviour();
    }

    private void CheckCurrentState()
    {
        if (currentState == null)
        {
            currentState = new IdleState(this, agentAISettings.CheckForCloseEnemyInIdlePeriod);
            currentState.Start();
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, agentSettings.LookDistance);
    }

    private void CurrentBehaviour()
    {
        currentState.Update();

        if (!currentState.IsStopped)
        {
            return;
        }

        TransferToNextState();
    }

    private void TransferToNextState()
    {
        if (currentState.GetNextStateType() == typeof(IdleState))
        {
            currentState = new IdleState(this, agentAISettings.CheckForCloseEnemyInIdlePeriod);
            currentState.Start();
        }
        else if (currentState.GetNextStateType() == typeof(MoveState))
        {
            /// For now nothing
            /// If going to this state without goal, need to pass destination,
            /// because without it agent will not go anywhere
            currentState = new MoveState(this, agentAISettings.CheckForCloseEnemyInMovePeriod);
            currentState.Start();
        }
        else if (currentState.GetNextStateType() == typeof(AttackState))
        {
            Type type = currentState.GetType();

            Transform visibleEnemyBodyPart = null;
            Agent agentToAim = null;

            if (type.GetMethod("GetVisibleEnemyBodyPart") != null)
            {
                visibleEnemyBodyPart = (Transform)type.GetMethod("GetVisibleEnemyBodyPart").Invoke(currentState, new object[] { });
            }

            if (type.GetMethod("GetAgentToAim") != null)
            {
                agentToAim = (Agent)type.GetMethod("GetAgentToAim").Invoke(currentState, new object[] { });
            }

            currentState = new AttackState(this,
                    agentAISettings.CheckForCloseEnemyInAttackPeriod,
                    visibleEnemyBodyPart,
                    agentToAim);

            currentState.Start();
        }
        else
        {
            currentState = null;
            Debug.Log("No next state, going to idle");
        }
    }
}
