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

    public virtual void Stop(Type nextStateType)
    {
        Stop();
        this.nextStateType = nextStateType;
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
    protected bool isEnemySurroundedFromTheBeginning = false;
    
    protected float checkForCloseEnemyPeriod = 1f;
    protected float timeSinceEnemySearch = 0f;
    protected Transform visibleEnemyBodyPart = null;

    protected Vector3 previosCheckCoordinate = new Vector3(Mathf.Infinity, Mathf.Infinity, Mathf.Infinity);
    protected float timeSinceLastEndPathCheck = 0f;
    #endregion

    public MoveState(Agent agent,
        Vector3 destination,
        float checkForCloseEnemyPeriod,
        Transform moveTransform = null) : base(agent)
    {
        this.destination = destination;
        this.moveTransform = moveTransform;
        this.checkForCloseEnemyPeriod = checkForCloseEnemyPeriod;
    }

    public MoveState(Agent agent,
        float checkForCloseEnemyPeriod,
        Transform moveTransform = null) : base(agent)
    {
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

        CheckEnemiesPresenceInLineOfSight();

        previosCheckCoordinate = new Vector3(Mathf.Infinity, Mathf.Infinity, Mathf.Infinity);
        timeSinceLastEndPathCheck = 0f;
        agent.GetAIPathHandler().isStopped = false;
    }

    private void CheckEnemiesPresenceInLineOfSight()
    {
        bool isEnemyFound = FindClosestEnemyOpenBodyPart();

        isEnemySurroundedFromTheBeginning = isEnemyFound;
    }

    public override void Stop()
    {
        base.Stop();
        // nextStateType = typeof(IdleState);
        agent.GetAIPathHandler().isStopped = true;
    }

    public override void Update()
    {
        if (isStopped)
        {
            return;
        }
        
        if (agent.GetCurrentGoal() is MoveGoal)
        {
            MoveGoalMovement();
        }
        else if (agent.GetCurrentGoal() is AttackGoal)
        {
            AttackGoalMovement();
        }
        else
        {
            NoGoalMovement();
        }
    }

    protected void NoGoalMovement()
    {
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
            Debug.Log("Reached destination!");
            return;
        }

        if (isEnemySurroundedFromTheBeginning)
        {
            return;
        }

        bool isEnemyFound = SearchForEnemies();

        if (isEnemyFound)
        {
            Stop(typeof(AttackState));
            Debug.Log("Enemy in line of sight!");
            return;
        }
    }

    protected void MoveGoalMovement()
    {
        RichAI aiPathHandler = agent.GetAIPathHandler();

        MoveGoal moveGoal = agent.GetCurrentGoal() as MoveGoal;

        aiPathHandler.destination = moveGoal.Destination;

        bool isEndPath = CheckEndPath();

        if (aiPathHandler.reachedDestination || isEndPath)
        {
            Stop();
            agent.ClearCurrentGoal();
            Debug.Log("Reached move goal!");
            return;
        }

        if (isEnemySurroundedFromTheBeginning)
        {
            return;
        }

        bool isEnemyFound = SearchForEnemies();

        if (isEnemyFound)
        {
            Stop(typeof(AttackState));
            Debug.Log("Enemy in line of sight!");
            return;
        }
    }

    protected void AttackGoalMovement()
    {
        RichAI aiPathHandler = agent.GetAIPathHandler();

        AttackGoal attackGoal = agent.GetCurrentGoal() as AttackGoal;

        if (attackGoal.AgentToAttack == null)
        {
            Stop(typeof(IdleState));
            agent.ClearCurrentGoal();
            Debug.Log("Attack goal completed, no agent to attack!");
            return;
        }

        aiPathHandler.destination = attackGoal.Destination;

        bool isEndPath = CheckEndPath();

        if (aiPathHandler.reachedDestination || isEndPath)
        {
            Stop(typeof(AttackState));
            Debug.Log("Reached attack goal!");
            return;
        }

        bool isEnemyReachable = IsTargetEnemyReachable();

        if (isEnemyReachable)
        {
            Stop(typeof(AttackState));
            Debug.Log("TARGET enemy in line of sight!");
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
        Unit enemyUnit = (agent.GetCurrentGoal() as AttackGoal).AgentToAttack.SoldierBasic;

        if (!eyeSightManager.IsEnemyAtLookDistance(enemyUnit, agent.GetLookDistance()))
        {
            return false;
        }

        var enemyColliderCostPair = eyeSightManager.GetUnitsVisibleBodyPart(enemyUnit);
        if (enemyColliderCostPair != null)
        {
            visibleEnemyBodyPart = enemyColliderCostPair.collider.transform;

            return true;
        }

        return false;
    }

    private bool SearchForEnemies()
    {
        // Timer check
        timeSinceEnemySearch += Time.deltaTime;
        if (timeSinceEnemySearch < LevelManager.Instance.AgentsSecondsTillCheckEndPath)
        {
            return false;
        }
        timeSinceEnemySearch = 0;
        //////////////

        return FindClosestEnemyOpenBodyPart();
    }

    private bool FindClosestEnemyOpenBodyPart()
    {
        EyeSightManager eyeSightManager = agent.GetEyeSightManager();
        Unit closestEnemy = eyeSightManager.GetClothestEnemyUnitInFieldOfView(agent.GetLookDistance(), agent.GetController().GetTeam());
        if (closestEnemy != null)
        {
            var enemyColliderCostPair = eyeSightManager.GetUnitsVisibleBodyPart(closestEnemy);
            if (enemyColliderCostPair != null)
            {
                visibleEnemyBodyPart = enemyColliderCostPair.collider.transform;

                return true;
            }
        }

        return false;
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

        if (CheckGoals())
        {
            Stop();
            return;
        }

        bool isEnemyFound = SearchForEnemies();

        if (isEnemyFound)
        {
            Stop(typeof(AttackState));
            Debug.Log("Enemy in line of sight!");
            return;
        }
    }

    private bool CheckGoals()
    {
        if (agent.GetCurrentGoal() is MoveGoal)
        {
            agent.CurrentState = new MoveState(
                agent,
                (agent.GetCurrentGoal() as MoveGoal).Destination,
                agent.CheckForCloseEnemyInMovePeriod
                );
            agent.CurrentState.Start();

            return true;
        }

        if (agent.GetCurrentGoal() is AttackGoal)
        {
            agent.CurrentState = new MoveState(
                agent,
                (agent.GetCurrentGoal() as AttackGoal).Destination,
                agent.CheckForCloseEnemyInAttackPeriod
                );
            agent.CurrentState.Start();

            return true;
        }

        return false;
    }

    private bool SearchForEnemies()
    {
        // Timer check
        timeSinceEnemySearch += Time.deltaTime;
        if (timeSinceEnemySearch < checkForCloseEnemyPeriod)
        {
            return false;
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

                return true;
            }
        }

        return false;
    }

    public override void Stop()
    {
        base.Stop();
        // nextStateType = typeof(AttackState);
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

    public AttackState(Agent agent, float checkForCloseEnemyPeriod, Transform visibleEnemyBodyPart = null) : base(agent)
    {
        this.visibleEnemyBodyPart = visibleEnemyBodyPart;
        this.checkForCloseEnemyPeriod = checkForCloseEnemyPeriod;
    }

    public override void Update()
    {
        if (isStopped)
        {
            return;
        }

        if (agent.GetCurrentGoal() is AttackGoal)
        {
            AttackGoalAttack();
        }
        else
        {
            NoGoalAttack();
        }
    }

    private void AttackGoalAttack()
    {
        AttackGoal attackGoal = agent.GetCurrentGoal() as AttackGoal;

        if (attackGoal.AgentToAttack == null)
        {
            agent.ClearCurrentGoal();
            Debug.Log("Attack goal completed, no agent to attack!");
            return;
        }

        if (!CheckGoalTargetVisibility())
        {
            agent.ClearCurrentGoal();
            Debug.Log("Attack goal completed, now in regular attack mode!");
            return;
        }

        TryAttackEnemy();
    }

    private void NoGoalAttack()
    {
        if (!CheckCurrentTargetVisibility())
        {
            Stop();
            return;
        }

        TryAttackEnemy();
    }

    private bool CheckGoalTargetVisibility()
    {
        // Timer check
        timeSinceEnemySearch += Time.deltaTime;
        if (timeSinceEnemySearch < checkForCloseEnemyPeriod)
        {
            return true;
        }
        timeSinceEnemySearch = 0;
        //////////////

        return FindTargetEnemyOpenBodyPart();
    }

    private bool FindTargetEnemyOpenBodyPart()
    {
        AttackGoal attackGoal = agent.GetCurrentGoal() as AttackGoal;

        EyeSightManager eyeSightManager = agent.GetEyeSightManager();

        Unit enemyUnit = attackGoal.AgentToAttack.SoldierBasic;

        if (!eyeSightManager.IsEnemyAtLookDistance(enemyUnit, agent.GetLookDistance()))
        {
            return false;
        }

        var enemyColliderCostPair = eyeSightManager.GetUnitsVisibleBodyPart(enemyUnit);

        if (enemyColliderCostPair != null)
        {
            if (enemyColliderCostPair.collider.transform != visibleEnemyBodyPart)
            {
                visibleEnemyBodyPart = enemyColliderCostPair.collider.transform;
            }
            return true;
        }

        return false;
    }

    protected virtual bool CheckCurrentTargetVisibility()
    {
        // Timer check
        timeSinceEnemySearch += Time.deltaTime;
        if (timeSinceEnemySearch < checkForCloseEnemyPeriod)
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
                }
                return true;
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
        // nextStateType = typeof(IdleState);
    }
}
