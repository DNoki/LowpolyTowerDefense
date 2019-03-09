using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 矢が接触したときのエフェクト
/// </summary>
[RequireComponent(typeof(ParticleSystem))]
public class ArrowContactEffect : MonoBehaviour, IPoolObject
{
    public ParticleSystem Particle = null;

    public bool IsReleased { get; set; }

    void IPoolObject.Initialize()
    {
        this.IsReleased = false;
    }
    void IPoolObject.Release()
    {
        this.IsReleased = true;
    }

    public void AfterTakedInitialize(Vector3 position, Vector3 direction)
    {
        this.transform.position = position;
        this.transform.LookAt(this.transform.position + direction);

        this.Particle.Play();
        StartCoroutine(ReleaseThis());
    }
    public IEnumerator ReleaseThis()
    {
        while (this.Particle.isPlaying)
            yield return null;
        GameObjectFactory.ArrowTouchEffectPoolObject.Release(this);
    }

    private void Awake()
    {
        if (!this.Particle) this.Particle = GetComponent<ParticleSystem>();
    }
}
