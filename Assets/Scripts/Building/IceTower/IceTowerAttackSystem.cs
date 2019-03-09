using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 氷塔攻撃システム
/// </summary>
[RequireComponent(typeof(IceTower))]
public class IceTowerAttackSystem : MonoBehaviour, IBuildingAttackSystem
{
    [SerializeField] private IceTower iceTower = null;
    [SerializeField] private Transform launcher = null;
    /// <summary>
    /// 攻击速度
    /// </summary>
    [SerializeField] private float attackSpeed = 3f;
    /// <summary>
    /// 攻击距离
    /// </summary>
    [SerializeField] private float attackDistance = 5f;
    /// <summary>
    /// 减速时长
    /// </summary>
    [SerializeField, Tooltip("减速时长")] private float decelerationTime = 2f;
    /// <summary>
    /// 减速强度（0~1）
    /// </summary>
    [SerializeField, Range(0, 1), Tooltip("减速强度（0~1）")] private float decelerationPower = 0.7f;
    /// <summary>
    /// 强度衰减曲线
    /// </summary>
    [SerializeField, Tooltip("强度衰减曲线")] private AnimationCurve attenuationCurve = new AnimationCurve();

    [SerializeField, Tooltip("攻速提升")] private float attackSpeedPromotion = 0.5f;
    [SerializeField, Tooltip("攻击距离提升")] private float attackDistancePromotion = 0.5f;
    [SerializeField, Tooltip("减速时长提升")] private float decelerationTimePromotion = 0.5f;
    [SerializeField, Range(0f, 0.4f), Tooltip("减速强度提升")] private float decelerationPowerPromotion = 0.2f;

    private Timer attackSpeedTimer = new Timer(3f);

    public float AttackSpeed => this.attackSpeed - (this.iceTower.CurrentLevel - 1) * this.attackSpeedPromotion;
    /// <summary>
    /// 攻击距离
    /// </summary>
    public float AttackDistance => this.attackDistance + (this.iceTower.CurrentLevel - 1) * this.attackDistancePromotion;
    public float DecelerationTime => this.decelerationTime + (this.iceTower.CurrentLevel - 1) * this.decelerationTimePromotion;
    public float DecelerationPower => this.decelerationPower - (this.iceTower.CurrentLevel - 1) * this.decelerationPowerPromotion;
    public AnimationCurve AttenuationCurve => this.attenuationCurve;

    /// <summary>
    /// 减速攻击
    /// </summary>
    public void Attack()
    {
        if (this.iceTower.BuildState == BuildState.PREVIEW) return;
        if (!Physics.CheckCapsule(this.transform.position.SetValue(y: -1000f), this.transform.position.SetValue(y: 1000f), this.AttackDistance, EnemyFactory.ENEMY_LAYER_MASK))
            return;
        var iceCircle = GameObjectFactory.IceCirclePoolObject.Take();
        iceCircle.AfterTakedInitialize(this.launcher.position, this.AttackDistance, this.DecelerationTime, this.DecelerationPower, this.AttenuationCurve);
    }

    private void Awake()
    {
        if (!this.iceTower) this.iceTower = GetComponent<IceTower>();

        if (this.attackSpeedTimer == null) this.attackSpeedTimer = new Timer(this.AttackSpeed);
        else this.attackSpeedTimer.DefiniteTime = this.AttackSpeed;
        this.attackSpeedTimer.AfterTime = Random.Range(0, this.attackSpeedTimer.DefiniteTime);
    }
    private void Update()
    {
        if (this.attackSpeedTimer.DefiniteTime != this.AttackSpeed)
            this.attackSpeedTimer.DefiniteTime = this.AttackSpeed;
        if (this.attackSpeedTimer.UpdateAndIsReach(Time.deltaTime))
            Attack();

    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        var circle = new Circle(this.iceTower.Effect.transform.position, this.AttackDistance);

        for (var i = 0; i < 20; i++)
        {
            var point1 = circle.CenterPoint + Quaternion.AngleAxis(i * 360f / 20f, Vector3.up) * Vector3.forward;
            var point2 = circle.CenterPoint + Quaternion.AngleAxis((i + 1) * 360f / 20f, Vector3.up) * Vector3.forward;

            Gizmos.DrawLine(circle.ClosestPoint(point1), circle.ClosestPoint(point2));
        }
    }
#endif
}
