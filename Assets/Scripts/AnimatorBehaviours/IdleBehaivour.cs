using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleBehaivour : StateMachineBehaviour
{
    [SerializeField] string intParameterName;
    [SerializeField] public int statesCount = 0;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetInteger(intParameterName, Random.Range(0, statesCount));
    }
}
