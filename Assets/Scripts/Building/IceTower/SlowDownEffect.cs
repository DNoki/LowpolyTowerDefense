using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowDownEffect : MonoBehaviour, IPoolObject
{
    [SerializeField] private List<ParticleSystem> loopPSs = null;
    [SerializeField] private List<ParticleSystem> snowPSs = null;

    public bool IsReleased { get; set; }
    public IEnemy Target { get; set; }

    void IPoolObject.Initialize()
    {
        this.IsReleased = false;
        for (var i = 0; i < this.loopPSs.Count; i++)
            this.loopPSs[i].Play();
        this.snowPSs[Random.Range(0, this.snowPSs.Count)].Play();
    }
    void IPoolObject.Release()
    {
        this.IsReleased = true;
    }
    public void AfterTakedInitialize(IEnemy target, float duration)
    {
        this.Target = target;
        StartCoroutine(ReleaseThis(duration));
    }

    /// <summary>
    /// 释放
    /// </summary>
    /// <param name="duration">持续时间</param>
    /// <returns></returns>
    private IEnumerator ReleaseThis(float duration)
    {
        while (true)
        {
            if (duration <= 0) break;
            if (this.Target.IsDie) break;

            this.transform.position = this.Target.Transform.position;

            yield return null;
            duration -= Time.deltaTime;
        }

        // 结束所有粒子
        for (var i = 0; i < Mathf.Max(this.loopPSs.Count, this.snowPSs.Count); i++)
        {
            if (i < this.loopPSs.Count)
                this.loopPSs[i].Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            if (i < this.snowPSs.Count)
                this.snowPSs[i].Stop();
        }

        // 当所有粒子显示结束后，再释放
        while (true)
        {
            this.transform.position = this.Target.Transform.position;

            var isContinue = false;
            for (var i = 0; i < Mathf.Max(this.loopPSs.Count, this.snowPSs.Count); i++)
            {
                //if (i < this.loopPSs.Count)
                //    if (this.loopPSs[i].particleCount > 0)
                //    {
                //        isContinue = true;
                //        break;
                //    }
                if (i < this.snowPSs.Count)
                    if (this.snowPSs[i].isPlaying)
                    {
                        isContinue = true;
                        break;
                    }
            }
            yield return null;
            if (isContinue) continue;
            else break;
        }
        GameObjectFactory.SlowDownEffectPoolObject.Release(this);
    }
}
