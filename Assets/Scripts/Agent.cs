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
    ////////////////////////////////////

    // Getters and setters
    #region
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

        if (newGoal is MoveByCommandGoal)
        {
            currentState = new MoveState(this, (newGoal as MoveByCommandGoal).Destination);
            currentState.Start();
        }
    }

    public Goal GetCurrentGoal()
    {
        return currentGoal;
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
            if (currentGoal is MoveGoal)
            {
                currentState = new MoveState(this, (currentGoal as MoveGoal).Destination);
                currentState.Start();
            }
            else if (currentGoal is MoveByCommandGoal)
            {
                currentState = new MoveState(this, (currentGoal as MoveByCommandGoal).Destination);
                currentState.Start();
            }
        }
        else if (currentState.GetNextStateType() == typeof(AttackState))
        {
            currentState = new AttackState(this, (currentState as IdleState)?.GetVisibleEnemyBodyPart());
            currentState.Start();
        }
        else
        {
            Debug.Log("No next state, going to idle");
        }
    }
}
