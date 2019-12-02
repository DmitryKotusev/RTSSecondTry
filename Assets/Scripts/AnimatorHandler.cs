using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class AnimatorHandler : MonoBehaviour
{
    [SerializeField]
    Animator animator;
    void Start()
    {
        Assert.IsNotNull(animator);
        animator.SetFloat("IdleCycleOffset", Random.Range(0f, 4f));
    }
}
