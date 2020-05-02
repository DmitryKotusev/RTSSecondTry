using UnityEngine;

public abstract class AILogic : ScriptableObject
{
    protected AIController aIController;

    abstract public void Init(AIController aIController);

    abstract public void Update();
}
