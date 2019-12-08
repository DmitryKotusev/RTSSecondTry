using UnityEngine;

abstract public class Goal
{
}

public class MoveGoal : Goal
{
    public Vector3 Destination
    {
        get; set;
    }
}

public class AttackGoal : Goal
{
    public Agent agentToAttack
    {
        get; set;
    }
}
