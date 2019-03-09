using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 矢塔の攻撃システム
/// </summary>
[RequireComponent(typeof(ArrowTower))]
public class ArrowTowerAttackSystem : MonoBehaviour, IBuildingAttackSystem
{
    [SerializeField] private ArrowTower tower = null;
    /// <summary>
    /// 攻击力
    /// 攻撃力
    /// </summary>
    [SerializeField] private float attackPower = 10f;
    /// <summary>
    /// 攻击速度
    /// 攻撃スピード
    /// </summary>
    [SerializeField] private float attackSpeed = 1.5f;
    /// <summary>
    /// 攻击距离
    /// 攻撃距離
    /// </summary>
    [SerializeField] private float attackDistance = 5f;
    /// <summary>
    /// 弓箭前进速度
    /// 矢の移動スピード
    /// </summary>
    [SerializeField] private float arrowSpeed = 10f;
    /// <summary>
    /// 发射位置与角度
    /// 発射位置と発射角度
    /// </summary>
    [SerializeField] private Transform[] launchers = null;

    /// <summary>
    /// 攻击力提升量
    /// 攻撃力増量
    /// </summary>
    [SerializeField] private float attackPowerPromotion = 8f;
    /// <summary>
    /// 攻速提升量
    /// 攻撃スピード増量
    /// </summary>
    [SerializeField] private float attackSpeedPromotion = 0.35f;
    /// <summary>
    /// 攻击范围提升
    /// 攻撃範囲増量
    /// </summary>
    [SerializeField] private float attackDistancePromotion = 5f;
    /// <summary>
    /// 弓箭前进速度
    /// 矢のスピード増量
    /// </summary>
    [SerializeField] private float arrowSpeedPromotion = 3f;

    private Timer attackSpeedTimer = new Timer(1.5f);

    public Transform Launcher => this.launchers[this.tower.CurrentLevel - 1];
    public IEnemy Target { get; set; }
    public DamageMessage GetDamageMessage => new DamageMessage(this.Launcher, this.AttackPower);
    public float AttackPower => this.attackPower + (this.tower.CurrentLevel - 1) * this.attackPowerPromotion;
    public float AttackSpeed => this.attackSpeed - (this.tower.CurrentLevel - 1) * this.attackSpeedPromotion;
    /// <summary>
    /// 攻击距离
    /// 攻撃距離
    /// </summary>
    public float AttackDistance => this.attackDistance + (this.tower.CurrentLevel - 1) * this.attackDistancePromotion;
    public float ArrowSpeed => this.arrowSpeed + (this.tower.CurrentLevel - 1) * this.arrowSpeedPromotion;

    public void Attack()
    {
        if (this.tower.BuildState == BuildState.PREVIEW) return;
        var hits = Physics.OverlapCapsule(this.transform.position.SetValue(y: -1000f), this.transform.position.SetValue(y: 1000f), this.AttackDistance, EnemyFactory.ENEMY_LAYER_MASK);
        var enemys = new List<IEnemy>();
        for (var i = 0; i < hits.Length; i++)
        {
            var enemy = hits[i].transform.GetComponent<IEnemy>();
            if (enemy == null || enemy.IsDie) continue;
            enemys.Add(enemy);

            if (enemy == this.Target)
            {
                var bullet = GameObjectFactory.ArrowPoolObject.Take();
                bullet.AfterTakedInitialize(this.Target, this.GetDamageMessage, this.ArrowSpeed, this.GetType());
                return;
            }
        }

        enemys.Sort((a, b) => a.SerialNumber.CompareTo(b.SerialNumber));

        if (enemys.Count > 0)
        {
            this.Target = enemys[0];
            var arrow = GameObjectFactory.ArrowPoolObject.Take();
            arrow.AfterTakedInitialize(this.Target, this.GetDamageMessage, this.ArrowSpeed, this.GetType());
        }
    }

    private void Awake()
    {
        if (!this.tower) this.tower = GetComponent<ArrowTower>();

        if (this.attackSpeedTimer == null) this.attackSpeedTimer = new Timer(this.AttackSpeed);
        else this.attackSpeedTimer.DefiniteTime = this.AttackSpeed;
        this.attackSpeedTimer.AfterTime = Random.Range(0, this.attackSpeedTimer.DefiniteTime);
    }
    private void Update()
    {
        // 攻击计时器定时与攻速不同则更新
        if (this.attackSpeedTimer.DefiniteTime != this.AttackSpeed)
            this.attackSpeedTimer.DefiniteTime = this.AttackSpeed;
        if (this.attackSpeedTimer.UpdateAndIsReach(Time.deltaTime))
            Attack();
        if (this.Target != null && this.Target.IsDie)
            this.Target = null;
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        var circle = new Circle(this.tower.Effect.transform.position, this.AttackDistance);

        for (var i = 0; i < 20; i++)
        {
            var point1 = circle.CenterPoint + Quaternion.AngleAxis(i * 360f / 20f, Vector3.up) * Vector3.forward;
            var point2 = circle.CenterPoint + Quaternion.AngleAxis((i + 1) * 360f / 20f, Vector3.up) * Vector3.forward;

            Gizmos.DrawLine(circle.ClosestPoint(point1), circle.ClosestPoint(point2));
        }
    }
#endif
}

