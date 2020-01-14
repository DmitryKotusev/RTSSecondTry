using System.Collections;
using UnityEngine;
using Sirenix.OdinInspector;
using Pathfinding;
using System;

public class Agent : MonoBehaviour
{
    [BoxGroup("Projectors")]
    [SerializeField]
    [Tooltip("Projector used to highlight if unit is in selection list")]
    GameObject selectionProjector;

    [BoxGroup("Projectors")]
    [SerializeField]
    [Tooltip("Projector used to highlight if unit is main in selection list")]
    GameObject mainSelectionProjector;

    [BoxGroup("Settings")]
    [SerializeField]
    [Tooltip("Agent's look distance")]
    float lookDistance = 60f;

    [SerializeField]
    [Tooltip("Controller that is able to give commands to this unit")]
    Controller controller;

    [SerializeField]
    [Tooltip("AI path handler")]
    RichAI aiPathHandler;

    [SerializeField]
    AgentWeaponManager weaponManager;

    [SerializeField]
    EyeSightManager eyeSightManager;

    [SerializeField]
    SoldierBasic soldierBasic;
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
    [BoxGroup("Attack settings")]
    [SerializeField]
    private float checkForCloseEnemyInAttackPeriod = 2f;
    [BoxGroup("Idle behaivor settings")]
    [SerializeField]
    private float checkForCloseEnemyInIdlePeriod = 0.5f;
    [BoxGroup("Idle behaivor settings")]
    [SerializeField]
    private float checkForCloseEnemyInMovePeriod = 0.5f;
    ////////////////////////////////////

    // Getters and setters
    #region
    public float CheckForCloseEnemyInAttackPeriod
    {
        get
        {
            return checkForCloseEnemyInAttackPeriod;
        }
    }

    public float CheckForCloseEnemyInIdlePeriod
    {
        get
        {
            return checkForCloseEnemyInIdlePeriod;
        }
    }

    public float CheckForCloseEnemyInMovePeriod
    {
        get
        {
            return checkForCloseEnemyInMovePeriod;
        }
    }

    public RichAI GetAIPathHandler()
    {
        return aiPathHandler;
    }

    public Team GetTeam()
    {
        return controller.GetTeam();
    }

    public EyeSightManager GetEyeSightManager()
    {
        return eyeSightManager;
    }

    public AgentWeaponManager GetWeaponManager()
    {
        return weaponManager;
    }

    public float GetLookDistance()
    {
        return lookDistance;
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

        // TODO appropriate state
        currentState.Stop();

        if (newGoal is MoveGoal)
        {
            currentState = new MoveState(
                this,
                (newGoal as MoveGoal).Destination,
                checkForCloseEnemyInMovePeriod
                );
            currentState.Start();
        }

        if (newGoal is AttackGoal)
        {
            currentState = new MoveState(
                this,
                (newGoal as AttackGoal).Destination,
                checkForCloseEnemyInAttackPeriod
                );
            currentState.Start();
        }
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

    // Selection mark methods
    #region
    public void MarkAsSelected()
    {
        selectionProjector.SetActive(true);
    }

    public void MarkAsMainSelected()
    {
        mainSelectionProjector.SetActive(true);
    }

    public void MarkAsUnselected()
    {
        selectionProjector.SetActive(false);
        mainSelectionProjector.SetActive(false);
    }
    #endregion

    private void Update()
    {
        CheckCurrentState();
        CurrentBehaviour();
    }

    private void CheckCurrentState()
    {
        if (currentState == null)
        {
            currentState = new IdleState(this, checkForCloseEnemyInIdlePeriod);
            currentState.Start();
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, lookDistance);
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
            currentState = new IdleState(this, checkForCloseEnemyInIdlePeriod);
            currentState.Start();
        }
        else if (currentState.GetNextStateType() == typeof(MoveState))
        {
            /// For now nothing
            /// If going to this state without goal, need to pass destination,
            /// because without it agent will not go anywhere
            currentState = new MoveState(this, checkForCloseEnemyInMovePeriod);
            currentState.Start();
        }
        else if (currentState.GetNextStateType() == typeof(AttackState))
        {
            Type type = currentState.GetType();

            if (type.GetMethod("GetVisibleEnemyBodyPart") != null)
            {
                currentState = new AttackState(this, (Transform)type.GetMethod("GetVisibleEnemyBodyPart").Invoke(currentState, new object[] { }));
                currentState.Start();
            }
        }
        else
        {
            currentState = null;
            Debug.Log("No next state, going to idle");
        }
    }
}
