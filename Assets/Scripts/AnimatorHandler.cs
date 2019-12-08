using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using Pathfinding;

public class AnimatorHandler : MonoBehaviour
{
    [SerializeField]
    Animator animator;

    [SerializeField]
    RichAI richAI;

    void Start()
    {
        Assert.IsNotNull(animator);
        SetRandomIdleAnimationOffset();
    }

    private void Update()
    {
        UpdateAnimatorVelocity();
    }

    private void SetRandomIdleAnimationOffset()
    {
        animator.SetFloat("IdleCycleOffset", Random.Range(0f, 4f));
    }

    private void UpdateAnimatorVelocity()
    {
        Vector3 velocity = richAI.velocity;
        Vector3 localVelocity = transform.InverseTransformDirection(velocity);

        animator.SetFloat("MovingSpeed", localVelocity.z);
    }
}
