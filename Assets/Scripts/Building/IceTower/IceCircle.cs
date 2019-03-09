using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 氷塔の攻撃
/// </summary>
[RequireComponent(typeof(Collider), typeof(Rigidbody))]
public class IceCircle : MonoBehaviour, IPoolObject
{
    [SerializeField] private Rigidbody rigid = null;
    [SerializeField] private ParticleSystem iceRangeEffect = null;
    /// <summary>
    /// 粒子缩放动画曲线
    /// </summary>
    [SerializeField] private AnimationCurve diffusCurve = null;
    /// <summary>
    /// 扩散速度
    /// </summary>
    [SerializeField] private float diffusSpeed = 1f;
    public Audio AudioEmission = null;

    public bool IsReleased { get; private set; }
    /// <summary>
    /// 攻击距离
    /// </summary>
    public float AttackDistance { get; set; }
    /// <summary>
    /// 减速时长
    /// </summary>
    public float Duration { get; set; }
    /// <summary>
    /// 减速强度（0~1）
    /// </summary>
    public float DecelerationPower { get; set; }
    /// <summary>
    /// 强度衰减曲线
    /// </summary>
    public AnimationCurve AttenuationCurve { get; set; }

    /// <summary>
    /// 扩散计时
    /// </summary>
    private Timer diffusSpeedTimer = new Timer(1f);


    void IPoolObject.Initialize()
    {
        this.IsReleased = false;
        this.diffusSpeedTimer.Reset();
        this.gameObject.SetActive(true);
    }
    void IPoolObject.Release()
    {
        this.IsReleased = true;
        this.gameObject.SetActive(false);
    }
    public void AfterTakedInitialize(Vector3 position, float attackDistance,  float decelerationTime, float decelerationPower, AnimationCurve attenuationCurve)
    {
        this.transform.position = position;
        this.AttackDistance = attackDistance;
        this.Duration = decelerationTime;
        this.DecelerationPower = decelerationPower;
        this.AttenuationCurve = attenuationCurve;
        this.AudioEmission.PlayRandomAudio();
    }

    private IEnumerator ReleaseThis()
    {
        this.IsReleased = true;
        while (this.iceRangeEffect.isPlaying)
        {
            yield return null;
        }
        GameObjectFactory.IceCirclePoolObject.Release(this);
    }

    private void Awake()
    {
        if (this.rigid == null) this.rigid = GetComponent<Rigidbody>();

        if (this.diffusSpeedTimer == null) this.diffusSpeedTimer = new Timer(this.diffusSpeed);
        else this.diffusSpeedTimer.DefiniteTime = this.diffusSpeed;
    }
    private void FixedUpdate()
    {
        if (this.IsReleased) return;

        var scale = this.diffusCurve.Evaluate(this.diffusSpeedTimer.AfterPercent) * this.AttackDistance;
        this.transform.localScale = new Vector3(scale, 1f, scale);
        this.iceRangeEffect.transform.localScale = new Vector3(scale, scale, 1f);

        this.diffusSpeedTimer.UpdateTimer(Time.fixedDeltaTime);
        if (this.diffusSpeedTimer.IsReach())
        {
            StartCoroutine(ReleaseThis());
            return;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (this.IsReleased) return;
        var enemy = other.GetComponent<IEnemy>();
        if (enemy == null) return;
        enemy.AddBuff(new SlowDownDebuff(this.DecelerationPower, this.Duration, this.AttenuationCurve));

        GameObjectFactory.SlowDownEffectPoolObject.Take().AfterTakedInitialize(enemy, this.Duration);
    }
}
