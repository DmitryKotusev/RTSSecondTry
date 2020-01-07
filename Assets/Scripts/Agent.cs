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

    // Moving goal additional help variables
    #region
    private Coroutine checkEndPathCoroutine = null;
    private Vector3 previosCheckCoordinate = new Vector3(Mathf.Infinity, Mathf.Infinity, Mathf.Infinity);
    #endregion

    /////////////Idle behavior//////////
    [BoxGroup("Idle behaivor settings")]
    [SerializeField]
    private float checkForCloseEnemyWhenThereIsTargetPeriod = 2f;
    [BoxGroup("Idle behaivor settings")]
    [SerializeField]
    private float checkForCloseEnemyWhenThereIsNoTargetPeriod = 0.5f;


    private Coroutine checkForCloseEnemyWhenThereIsTarget = null;
    private bool isCheckForEnemyWhenThereIsTargetTurnedOn = false;
    private Coroutine checkForCloseEnemyWhenThereIsNoTarget = null;
    private bool isCheckForEnemyWhenThereIsNoTargetTurnedOn = false;
    private Transform currentEnemyUnitBodyPart = null;
    ////////////////////////////////////

    // Getters and setters
    #region
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
        if (currentGoal == null)
        {
            CleanUpIdleStateVariables();
        }

        currentGoal = newGoal;

        if (currentGoal is MoveGoal)
        {
            aiPathHandler.destination = (currentGoal as MoveGoal).Destination;
            aiPathHandler.isStopped = false;

            checkEndPathCoroutine = StartCoroutine(CheckEndPathAsync());
        }
    }

    private void CleanUpIdleStateVariables()
    {
        if (checkForCloseEnemyWhenThereIsTarget != null)
        {
            StopCoroutine(checkForCloseEnemyWhenThereIsTarget);
        }
        if (checkForCloseEnemyWhenThereIsNoTarget != null)
        {
            StopCoroutine(checkForCloseEnemyWhenThereIsNoTarget);
        }
        isCheckForEnemyWhenThereIsNoTargetTurnedOn = false;
        isCheckForEnemyWhenThereIsTargetTurnedOn = false;

        currentEnemyUnitBodyPart = null;
        if (weaponManager.AgentAimManager.IsAiming)
        {
            weaponManager.AgentAimManager.StopAiming();
            weaponManager.AgentAimManager.ClearTarget();
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
        CurrentBehaviour();
    }

    private void CurrentBehaviour()
    {
        if (currentGoal != null)
        {
            if (currentGoal is MoveGoal)
            {
                MoveToDestination((MoveGoal)currentGoal);
            }
            else if (currentGoal is AttackByCommandGoal)
            {
                AttackAgent((AttackByCommandGoal)currentGoal);
            }
            else
            {
                currentGoal = null;
            }
        }
        else
        {
            SearchForEnemies();
        }
    }

    private void AttackAgent(AttackByCommandGoal attackGoal)
    {
        Debug.Log("Try to attack someone");
    }

    private void MoveToDestination(MoveGoal moveGoal)
    {
        aiPathHandler.destination = moveGoal.Destination;

        if (aiPathHandler.reachedDestination)
        {
            aiPathHandler.isStopped = true;
            currentGoal = null;
            StopCoroutine(checkEndPathCoroutine);
            Debug.Log("Reached move goal!");
        }
    }

    private void SearchForEnemies()
    {
        if (currentEnemyUnitBodyPart == null)
        {
            if (!isCheckForEnemyWhenThereIsNoTargetTurnedOn)
            {
                checkForCloseEnemyWhenThereIsNoTarget
                    = StartCoroutine(CheckForCloseEnemyAsync(checkForCloseEnemyWhenThereIsNoTargetPeriod));
                isCheckForEnemyWhenThereIsNoTargetTurnedOn = true;
            }

            if (weaponManager.AgentAimManager.IsAiming)
            {
                weaponManager.AgentAimManager.StopAiming();
                weaponManager.AgentAimManager.ClearTarget();
            }

            return;
        }

        if (!isCheckForEnemyWhenThereIsTargetTurnedOn)
        {
            checkForCloseEnemyWhenThereIsTarget
                = StartCoroutine(CheckForCloseEnemyAsync(checkForCloseEnemyWhenThereIsTargetPeriod));
            isCheckForEnemyWhenThereIsTargetTurnedOn = true;
        }

        if (!weaponManager.AgentAimManager.IsAiming)
        {
            weaponManager.AgentAimManager.StartAiming();
        }

        weaponManager.AgentAimManager.SetTarget(currentEnemyUnitBodyPart);

        if (weaponManager.AgentAimManager.IsTargetReachable(currentEnemyUnitBodyPart))
        {
            weaponManager.ActiveGun.Fire();
        }
    }

    IEnumerator CheckForCloseEnemyAsync(float period)
    {
        while (true)
        {
            Unit closestEnemy = eyeSightManager.GetClothestEnemyUnitInFieldOfView(lookDistance, controller.GetTeam());
            if (closestEnemy != null)
            {
                var enemyColliderCostPair = eyeSightManager.GetUnitsVisibleBodyPart(closestEnemy);
                if (enemyColliderCostPair != null)
                {
                    if (enemyColliderCostPair.collider.transform != currentEnemyUnitBodyPart)
                    {
                        currentEnemyUnitBodyPart = enemyColliderCostPair.collider.transform;
                        isCheckForEnemyWhenThereIsNoTargetTurnedOn = false;
                        isCheckForEnemyWhenThereIsTargetTurnedOn = false;
                        if (checkForCloseEnemyWhenThereIsTarget != null)
                        {
                            StopCoroutine(checkForCloseEnemyWhenThereIsTarget);
                        }
                        if (checkForCloseEnemyWhenThereIsNoTarget != null)
                        {
                            StopCoroutine(checkForCloseEnemyWhenThereIsNoTarget);
                        }
                        break;
                    }
                }
            }

            if (currentEnemyUnitBodyPart != null)
            {
                CheckMainTarget();
            }

            yield return new WaitForSeconds(period);
        }
    }

    private void CheckMainTarget()
    {
        Unit currentUnit = currentEnemyUnitBodyPart.GetComponent<Collider>().attachedRigidbody.GetComponent<Unit>();
        if (eyeSightManager.GetUnitsVisibleBodyPart(currentUnit) == null)
        {
            currentEnemyUnitBodyPart = null;
            isCheckForEnemyWhenThereIsTargetTurnedOn = false;
            if (checkForCloseEnemyWhenThereIsTarget != null)
            {
                StopCoroutine(checkForCloseEnemyWhenThereIsTarget);
            }
        }
    }

    IEnumerator CheckEndPathAsync()
    {
        previosCheckCoordinate = transform.position;
        yield return new WaitForSeconds(LevelManager.Instance.AgentsSecondsTillCheckEndPath);

        while ((previosCheckCoordinate - transform.position).magnitude > Mathf.Epsilon)
        {
            previosCheckCoordinate = transform.position;
            yield return new WaitForSeconds(LevelManager.Instance.AgentsSecondsTillCheckEndPath);
        }

        if (currentGoal is MoveGoal)
        {
            aiPathHandler.isStopped = true;
            currentGoal = null;
            Debug.Log("Reached move goal!"); ;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, lookDistance);
    }
}
