using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BakuhatuEffect : MonoBehaviour, IPoolObject
{
    public Audio AudioBakuhatu = null;
    public ParticleSystem Particle = null;

    public bool IsReleased { get; private set; }

    void IPoolObject.Initialize()
    {
        this.IsReleased = false;
        this.AudioBakuhatu.PlayRandomAudio();
        this.Particle.Play();
    }
    void IPoolObject.Release()
    {
        this.IsReleased = true;
    }
    public BakuhatuEffect AfterTakedInitialize(Vector3 position)
    {
        this.transform.position = position;
        StartCoroutine(ReleaseThis(5f));
        return this;
    }
    public IEnumerator ReleaseThis(float time)
    {
        yield return new WaitForSecondsRealtime(time);
        GameObjectFactory.BakuhatuEffectPoolObject.Release(this);
    }
}
