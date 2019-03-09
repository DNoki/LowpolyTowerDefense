using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 矢が接触したときのエフェクト
/// </summary>
[RequireComponent(typeof(ParticleSystem))]
public class ArrowContactFlashEffect : MonoBehaviour, IPoolObject
{
    public ParticleSystem Particle = null;

    public Transform Target { get; set; }
    public bool IsReleased { get; set; }

    void IPoolObject.Initialize()
    {
        this.IsReleased = false;
        this.Target = null;
    }
    void IPoolObject.Release()
    {
        this.IsReleased = true;
        this.Target = null;
    }

    public void AfterTakedInitialize(Vector3 position, Transform target)
    {
        this.transform.position = position;
        this.Target = target;

        this.Particle.Play();
        StartCoroutine(ReleaseThis());
    }
    public IEnumerator ReleaseThis()
    {
        while (this.Particle.isPlaying)
        {
            this.transform.position = this.Target.position;
            yield return null;
        }
        GameObjectFactory.ArrowContactFlashEffectPoolObject.Release(this);
    }

    private void Awake()
    {
        if (!this.Particle) this.Particle = GetComponent<ParticleSystem>();
    }
}
