using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class ShotEffect : MonoBehaviour
{
    [SerializeField]
    AudioSource audioSource;

    [SerializeField]
    List<ParticleSystem> particleSystems = new List<ParticleSystem>();

    Coroutine selfDestroyCoroutine;

    private void Start()
    {
        Assert.IsNotNull(audioSource);
        Assert.IsNotNull(audioSource?.clip);
    }

    public void ShowEffect()
    {
        List<float> durations = new List<float>();

        durations.Add(audioSource.clip.length);

        audioSource.Play();

        foreach (var particleSystem in particleSystems)
        {
            durations.Add(particleSystem.main.duration);
            particleSystem.Play();
        }

        selfDestroyCoroutine = StartCoroutine(SelfDestroy(durations));
    }

    IEnumerator SelfDestroy(List<float> durations)
    {
        float maxDuration = Mathf.Max(durations.ToArray());
        yield return new WaitForSeconds(maxDuration);

        PooledObject pooledObject = GetComponent<PooledObject>();
        pooledObject.pool.ReturnObject(gameObject);
    }
}
