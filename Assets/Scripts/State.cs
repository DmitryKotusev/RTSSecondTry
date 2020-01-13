using UnityEngine;
using Sirenix.OdinInspector;
using Pathfinding;
using System.Collections;
using System;

abstract public class State
{
    protected Agent agent;
    protected bool isStopped = true;
    protected Type nextStateType = null;

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

    public virtual Type GetNextStateType()
    {
        return nextStateType;
    }
}

public class MoveState : State
{
    protected Transform moveTransform = null;
    protected Vector3 destination;

    // Moving state additional help variables
    #region
    protected float checkForCloseEnemyPeriod = 1f;
    protected float timeSinceEnemySearch = 0f;
    protected Transform visibleEnemyBodyPart = null;

    protected Vector3 previosCheckCoordinate = new Vector3(Mathf.Infinity, Mathf.Infinity, Mathf.Infinity);
    protected float timeSinceLastEndPathCheck = 0f;
    #endregion

    public MoveState(Agent agent, Vector3 destination, float checkForCloseEnemyPeriod, Transform moveTransform = null) : base(agent)
    {
        this.destination = destination;
        this.moveTransform = moveTransform;
        this.checkForCloseEnemyPeriod = checkForCloseEnemyPeriod;
    }

    public Transform GetVisibleEnemyBodyPart()
    {
        return visibleEnemyBodyPart;
    }

    public override void Start()
    {
        base.Start();
        previosCheckCoordinate = new Vector3(Mathf.Infinity, Mathf.Infinity, Mathf.Infinity);
        timeSinceLastEndPathCheck = 0f;
        agent.GetAIPathHandler().isStopped = false;
    }

    public override void Stop()
    {
        base.Stop();
        nextStateType = typeof(IdleState);
        agent.GetAIPathHandler().isStopped = true;
    }

    public override void Update()
    {
        if (isStopped)
        {
            return;
        }
        RichAI aiPathHandler = agent.GetAIPathHandler();

        if (moveTransform != null)
        {
            aiPathHandler.destination = moveTransform.position;
        }
        else
        {
            aiPathHandler.destination = destination;
        }

        bool isEndPath = CheckEndPath();

        if (aiPathHandler.reachedDestination || isEndPath)
        {
            Stop();
            Debug.Log("Reached move goal!");
        }
    }

    protected bool CheckEndPath()
    {
        // Timer check
        timeSinceLastEndPathCheck += Time.deltaTime;
        if (timeSinceLastEndPathCheck < LevelManager.Instance.AgentsSecondsTillCheckEndPath)
        {
            return false;
        }
        timeSinceLastEndPathCheck = 0;
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
    private float timeSinceEnemySearch = 0f;
    private Transform visibleEnemyBodyPart = null;
    #endregion

    public IdleState(Agent agent, float checkForCloseEnemyPeriod) : base(agent)
    {
        this.checkForCloseEnemyPeriod = checkForCloseEnemyPeriod;
    }

    public Transform GetVisibleEnemyBodyPart()
    {
        return visibleEnemyBodyPart;
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
        timeSinceEnemySearch += Time.deltaTime;
        if (timeSinceEnemySearch < LevelManager.Instance.AgentsSecondsTillCheckEndPath)
        {
            return;
        }
        timeSinceEnemySearch = 0;
        //////////////
        
        EyeSightManager eyeSightManager = agent.GetEyeSightManager();
        Unit closestEnemy = eyeSightManager.GetClothestEnemyUnitInFieldOfView(agent.GetLookDistance(), agent.GetController().GetTeam());
        if (closestEnemy != null)
        {
            var enemyColliderCostPair = eyeSightManager.GetUnitsVisibleBodyPart(closestEnemy);
            if (enemyColliderCostPair != null)
            {
                visibleEnemyBodyPart = enemyColliderCostPair.collider.transform;

                Stop();
            }
        }
    }

    public override void Stop()
    {
        base.Stop();
        nextStateType = typeof(AttackState);
    }
}

public class AttackState : State
{
    // Idle state additional help variables
    #region
    protected float checkForCloseEnemyPeriod = 1f;
    protected float timeSinceEnemySearch = 0f;
    protected Transform visibleEnemyBodyPart = null;
    #endregion

    public AttackState(Agent agent, Transform visibleEnemyBodyPart = null) : base(agent)
    {
        this.visibleEnemyBodyPart = visibleEnemyBodyPart;
    }

    public override void Update()
    {
        if (isStopped)
        {
            return;
        }

        if (!CheckCurrentTargetVisibility())
        {
            Stop();
            return;
        }

        TryAttackEnemy();
    }

    protected virtual bool CheckCurrentTargetVisibility()
    {
        // Timer check
        timeSinceEnemySearch += Time.deltaTime;
        if (timeSinceEnemySearch < LevelManager.Instance.AgentsSecondsTillCheckEndPath)
        {
            return true;
        }
        timeSinceEnemySearch = 0;
        //////////////

        return FindClosestEnemyOpenBodyPart();
    }

    protected virtual bool FindClosestEnemyOpenBodyPart()
    {
        EyeSightManager eyeSightManager = agent.GetEyeSightManager();

        Unit closestEnemy = eyeSightManager.GetClothestEnemyUnitInFieldOfView(agent.GetLookDistance(), agent.GetTeam());
        if (closestEnemy != null)
        {
            var enemyColliderCostPair = eyeSightManager.GetUnitsVisibleBodyPart(closestEnemy);
            if (enemyColliderCostPair != null)
            {
                if (enemyColliderCostPair.collider.transform != visibleEnemyBodyPart)
                {
                    visibleEnemyBodyPart = enemyColliderCostPair.collider.transform;
                    return true;
                }
            }
        }

        return false;
    }

    protected void TryAttackEnemy()
    {
        AgentWeaponManager weaponManager = agent.GetWeaponManager();
        if (!weaponManager.AgentAimManager.IsAiming)
        {
            weaponManager.AgentAimManager.StartAiming();
        }

        if (visibleEnemyBodyPart == null && !FindClosestEnemyOpenBodyPart())
        {
            Stop();
            return;
        }

        weaponManager.AgentAimManager.SetTarget(visibleEnemyBodyPart);

        if (weaponManager.AgentAimManager.IsTargetReachable(visibleEnemyBodyPart))
        {
            weaponManager.ActiveGun.Fire();
        }
    }

    public override void Stop()
    {
        base.Stop();

        AgentWeaponManager weaponManager = agent.GetWeaponManager();
        weaponManager.AgentAimManager.StopAiming();
        weaponManager.AgentAimManager.ClearTarget();
        nextStateType = typeof(IdleState);
    }
}

public class MoveToAttackState : MoveState
{
    Agent agentToAttack;

    public MoveToAttackState(Agent agent, Vector3 destination, Agent agentToAttack, float checkForCloseEnemyPeriod, Transform moveTransform = null)
        : base(agent, destination, checkForCloseEnemyPeriod, moveTransform)
    {
        this.agentToAttack = agentToAttack;
    }

    public override void Start()
    {
        base.Start();

        if (IsTargetEnemyReachable())
        {
            Stop();
        }
    }

    public override void Stop()
    {
        base.Stop();

        nextStateType = typeof(AttackCertainAgentState);
    }

    public override void Update()
    {
        if (isStopped)
        {
            return;
        }

        if (agentToAttack == null)
        {
            Stop();
            return;
        }

        RichAI aiPathHandler = agent.GetAIPathHandler();

        if (moveTransform != null)
        {
            aiPathHandler.destination = moveTransform.position;
        }
        else
        {
            aiPathHandler.destination = destination;
        }

        bool isEndPath = CheckEndPath();

        if (aiPathHandler.reachedDestination || isEndPath)
        {
            Stop();
            Debug.Log("Reached move goal!");
            return;
        }

        bool isEnemyReachable = IsTargetEnemyReachable();

        if (isEnemyReachable)
        {
            Stop();
            Debug.Log("Enemy reached!");
            return;
        }
    }

    private bool IsTargetEnemyReachable()
    {
        // Timer check
        timeSinceEnemySearch += Time.deltaTime;
        if (timeSinceEnemySearch < LevelManager.Instance.AgentsSecondsTillCheckEndPath)
        {
            return false;
        }
        timeSinceEnemySearch = 0;
        //////////////

        EyeSightManager eyeSightManager = agent.GetEyeSightManager();
        Unit enemyUnit = agent.SoldierBasic;
        var enemyColliderCostPair = eyeSightManager.GetUnitsVisibleBodyPart(enemyUnit);
        if (enemyColliderCostPair != null)
        {
            visibleEnemyBodyPart = enemyColliderCostPair.collider.transform;

            return true;
        }

        return false;
    }

    

}

public class AttackCertainAgentState : AttackState
{
    Agent agentToAttack;

    public AttackCertainAgentState(Agent agent, Agent agentToAttack, Transform visibleEnemyBodyPart = null) : base(agent, visibleEnemyBodyPart)
    {
        this.agentToAttack = agentToAttack;
    }

    public override void Update()
    {
        if (isStopped)
        {
            return;
        }

        if (agentToAttack == null)
        {
            Stop();
            return;
        }

        if (!CheckCurrentTargetVisibility())
        {
            Stop();
            return;
        }

        TryAttackEnemy();
    }

    public override void Stop()
    {
        base.Stop();
        nextStateType = typeof(IdleState);
    }

    protected override bool CheckCurrentTargetVisibility()
    {
        // Timer check
        timeSinceEnemySearch += Time.deltaTime;
        if (timeSinceEnemySearch < LevelManager.Instance.AgentsSecondsTillCheckEndPath)
        {
            return true;
        }
        timeSinceEnemySearch = 0;
        //////////////

        return FindTargetEnemyOpenBodyPart();
    }

    private bool FindTargetEnemyOpenBodyPart()
    {
        EyeSightManager eyeSightManager = agent.GetEyeSightManager();

        Unit enemyUnit = agentToAttack.SoldierBasic;

        var enemyColliderCostPair = eyeSightManager.GetUnitsVisibleBodyPart(enemyUnit);

        if (enemyColliderCostPair != null)
        {
            if (enemyColliderCostPair.collider.transform != visibleEnemyBodyPart)
            {
                visibleEnemyBodyPart = enemyColliderCostPair.collider.transform;
                return true;
            }
        }

        return false;
    }
}