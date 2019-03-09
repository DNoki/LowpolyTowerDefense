using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ドロップ金のエフェクト
/// </summary>
public class CoinEffect : MonoBehaviour, IPoolObject
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

    public void Play(Vector3 position, int minQuantity, int maxQuantity)
    {
        this.transform.position = position.SetValue(null, this.transform.position.y, null);

        var burst = this.Particle.emission.GetBurst(0);
        burst.minCount = (short)minQuantity;
        burst.maxCount = (short)maxQuantity;

        this.Particle.emission.SetBurst(0, burst);
        this.Particle.Play();
        StartCoroutine(ReleaseThis());
    }
    public IEnumerator ReleaseThis()
    {
        while (this.Particle.isPlaying)
            yield return null;
        GameObjectFactory.CoinEffectPoolObject.Release(this);
    }

    private void Awake()
    {
        if (!this.Particle) this.Particle = GetComponent<ParticleSystem>();
    }
}

