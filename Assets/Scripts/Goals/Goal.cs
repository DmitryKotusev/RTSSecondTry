﻿using UnityEngine;

abstract public class Goal
{
    public Goal()
    {

    }
}

public class MoveGoal : Goal
{
    public Vector3 Destination
    {
        get; set;
    }

    public MoveGoal() : base()
    {
    }

    public MoveGoal(Vector3 destination)
    {
        Destination = destination;
    }
}

/// <summary>
/// Supposed to be set only by players
/// </summary>
public class AttackGoal : Goal
{
    public Agent AgentToAttack
    {
        get; set;
    }

    public Vector3 Destination
    {
        get; set;
    }

    public AttackGoal(Agent agentToAttack, Vector3 destination)
    {
        AgentToAttack = agentToAttack;
        Destination = destination;
    }
}
