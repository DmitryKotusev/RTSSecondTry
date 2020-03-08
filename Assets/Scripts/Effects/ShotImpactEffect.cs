using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class ShotImpactEffect : Effect
{
    [SerializeField]
    [Required]
    AudioSource audioSource;

    [SerializeField]
    List<ParticleSystem> particleSystems = new List<ParticleSystem>();

    [SerializeField]
    List<AudioClip> clipsToPlay = new List<AudioClip>();

    Coroutine selfDestroyCoroutine;

    public override void ShowEffect()
    {
        List<float> durations = new List<float>();

        AudioClip clipToPlay = ChooseRandomClipToPlay();

        durations.Add(clipToPlay.length);

        audioSource.clip = clipToPlay;

        foreach (var particleSystem in particleSystems)
        {
            durations.Add(particleSystem.main.duration);
        }

        selfDestroyCoroutine = StartCoroutine(SelfDestroy(durations));
    }

    private AudioClip ChooseRandomClipToPlay()
    {
        int clipToChoose = Random.Range(0, clipsToPlay.Count);

        return clipsToPlay[clipToChoose];
    }

    private void OnEnable()
    {
        ShowEffect();
    }
}
