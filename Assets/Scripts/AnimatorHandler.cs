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

    public readonly int NoWeaponsStandLayerIndex = 0;
    public readonly int BothHandsRiffleWeaponLayerIndex = 1;
    public readonly int FullBodyRifleAimStandLayer = 2;
    public readonly int UpperBodyReloadLayer = 3;

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

    public void PlayAnimation(string stateName, int layer)
    {
        animator.Play(stateName, layer, 0);
    }

    public void SetReloadAnimationMultiplier(float speedMultiplier)
    {
        animator.SetFloat("ReloadAnimationMultiplier", speedMultiplier);
    }

    public void UpdateLayerWeight(int layerIndex, float weight)
    {
        float newWeight = Mathf.Clamp01(weight);

        animator.SetLayerWeight(layerIndex, newWeight);
    }

    public float GetLayersWeight(int layerIndex)
    {
        return animator.GetLayerWeight(layerIndex);
    }
}
