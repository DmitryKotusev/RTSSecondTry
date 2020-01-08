using UnityEngine;
using Sirenix.OdinInspector;
using Pathfinding;
using System.Collections;
using System;

abstract public class State
{
    protected Agent agent;
    protected bool isStopped = true;

    public State(Agent agent)
    {
        this.agent = agent;
    }

    public virtual void Start()
    {
        isStopped = false;
    }

    public virtual void Stop()
    {
        isStopped = true;
    }

    public bool IsStopped
    {
        get
        {
            return isStopped;
        }
    }

    public abstract void Update();
}

public class MoveState : State
{
    MoveGoal moveGoal;

    // Moving state additional help variables
    #region
    private Vector3 previosCheckCoordinate = new Vector3(Mathf.Infinity, Mathf.Infinity, Mathf.Infinity);
    private float timeSinceLastEnemySearch = 0f;
    #endregion

    public MoveState(MoveGoal moveGoal, Agent agent) : base(agent)
    {
        this.moveGoal = moveGoal;
    }

    public override void Start()
    {
        base.Start();
        previosCheckCoordinate = new Vector3(Mathf.Infinity, Mathf.Infinity, Mathf.Infinity);
        timeSinceLastEnemySearch = 0f;
    }

    public override void Stop()
    {
        base.Stop();
        agent.GetAIPathHandler().isStopped = true;
    }

    public override void Update()
    {
        if (isStopped)
        {
            return;
        }
        RichAI aiPathHandler = agent.GetAIPathHandler();
        aiPathHandler.destination = moveGoal.Destination;

        bool isEndPath = CheckEndPath();

        if (aiPathHandler.reachedDestination || isEndPath)
        {
            Stop();
            Debug.Log("Reached move goal!");
        }
    }

    private bool CheckEndPath()
    {
        // Timer check
        timeSinceLastEnemySearch += Time.deltaTime;
        if (timeSinceLastEnemySearch < LevelManager.Instance.AgentsSecondsTillCheckEndPath)
        {
            return false;
        }
        timeSinceLastEnemySearch = 0;
        //////////////

        if ((previosCheckCoordinate - agent.transform.position).magnitude > Mathf.Epsilon)
        {
            previosCheckCoordinate = agent.transform.position;
            return false;
        }

        return true;
    }
}

public class IdleState : State
{
    // Idle state additional help variables
    #region
    private float checkForCloseEnemyPeriod = 1f;
    private float timeSinceLastEndPathCheck = 0f;
    #endregion
    public IdleState(Agent agent, float checkForCloseEnemyPeriod) : base(agent)
    {
        this.checkForCloseEnemyPeriod = checkForCloseEnemyPeriod;
    }

    public override void Update()
    {
        if (isStopped)
        {
            return;
        }

        SearchForEnemies();
    }

    private void SearchForEnemies()
    {
        // Timer check
        timeSinceLastEndPathCheck += Time.deltaTime;
        if (timeSinceLastEndPathCheck < LevelManager.Instance.AgentsSecondsTillCheckEndPath)
        {
            return;
        }
        timeSinceLastEndPathCheck = 0;
        //////////////
        
        EyeSightManager eyeSightManager = agent.GetEyeSightManager();
        Unit closestEnemy = eyeSightManager.GetClothestEnemyUnitInFieldOfView(agent.GetLookDistance(), agent.GetController().GetTeam());
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
    }
}
