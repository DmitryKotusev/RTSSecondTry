using UnityEngine;

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

public class AttackGoal : Goal
{
    public Agent agentToAttack
    {
        get; set;
    }
}
