using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmunitionSmokeEffect : MonoBehaviour, IPoolObject
{
    [SerializeField] private ParticleSystem particle = null;

    public bool IsReleased { get; set; }

    void IPoolObject.Initialize()
    {
        this.IsReleased = false;
        this.particle.Play();
    }
    void IPoolObject.Release()
    {
        this.IsReleased = true;
    }
    public void AfterTakedInitialize(Vector3 position)
    {
        this.transform.position = position;
        StartCoroutine(ReleaseThis());
    }

    private IEnumerator ReleaseThis()
    {
        while (this.particle.isPlaying)
        {
            yield return null;
        }
        GameObjectFactory.AmmunitionSmokeEffectPoolObject.Release(this);
    }

    private void Awake()
    {
        if (!this.particle) this.particle = GetComponent<ParticleSystem>();
    }
}
