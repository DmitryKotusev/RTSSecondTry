using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using Sirenix.OdinInspector;

public class ShotEffect : Effect
{
    [SerializeField]
    [Required]
    AudioSource audioSource;

    [SerializeField]
    List<ParticleSystem> particleSystems = new List<ParticleSystem>();

    Coroutine selfDestroyCoroutine;

    private void Start()
    {
        Assert.IsNotNull(audioSource);
        Assert.IsNotNull(audioSource?.clip);
    }

    private void OnEnable()
    {
        ShowEffect();
    }

    public override void ShowEffect()
    {
        List<float> durations = new List<float>();

        durations.Add(audioSource.clip.length);

        foreach (var particleSystem in particleSystems)
        {
            durations.Add(particleSystem.main.duration);
        }

        selfDestroyCoroutine = StartCoroutine(SelfDestroy(durations));
    }
}
