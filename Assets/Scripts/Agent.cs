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

    public EyeSightManager GetEyeSightManager()
    {
        return eyeSightManager;
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

    /*private void SearchForEnemies()
    {
        if (currentEnemyUnitBodyPart == null)
        {
            if (!isCheckForEnemyWhenThereIsNoTargetTurnedOn)
            {
                checkForCloseEnemyWhenThereIsNoTarget
                    = StartCoroutine(CheckForCloseEnemyAsync(checkForCloseEnemyInIdlePeriod));
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
                = StartCoroutine(CheckForCloseEnemyAsync(checkForCloseEnemyInAttackPeriod));
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
    }*/

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

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, lookDistance);
    }
}
