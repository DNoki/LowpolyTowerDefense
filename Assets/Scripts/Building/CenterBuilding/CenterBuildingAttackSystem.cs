using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 建筑攻击系统
/// センター攻撃システム
/// </summary>
[RequireComponent(typeof(CenterBuilding))]
public class CenterBuildingAttackSystem : MonoBehaviour, IBuildingAttackSystem
{
    [SerializeField] private CenterBuilding centerBuilding = null;
    /// <summary>
    /// 攻击力
    /// </summary>
    [SerializeField] private float attackPower = 2f;
    /// <summary>
    /// 攻击速度
    /// </summary>
    [SerializeField] private float attackSpeed = 2f;
    /// <summary>
    /// 攻击距离
    /// </summary>
    [SerializeField] private float attackDistance = 20f;
    /// <summary>
    /// 弓箭前进速度
    /// </summary>
    [SerializeField] private float arrowSpeed = 10f;
    /// <summary>
    /// 发射位置与角度
    /// </summary>
    [SerializeField] private Transform[] launchers = null;

    /// <summary>
    /// 攻击力提升量
    /// </summary>
    [SerializeField, Tooltip("攻击力提升量")] private float attackPowerPromotion = 1;
    /// <summary>
    /// 来自回合的攻击力提升
    /// </summary>
    [SerializeField, Tooltip("来自回合的攻击力提升")] private float attackPowerPromotionForRound = 1;
    /// <summary>
    /// 攻速提升量
    /// </summary>
    [SerializeField] private float attackSpeedPromotion = 0.5f;
    /// <summary>
    /// 攻击范围提升
    /// </summary>
    [SerializeField] private float attackDistancePromotion = 5f;
    /// <summary>
    /// 弓箭前进速度
    /// </summary>
    [SerializeField] private float arrowSpeedPromotion = 3f;

    private Timer attackSpeedTimer = new Timer(1f);

    public Transform Launcher => this.launchers[this.centerBuilding.CurrentLevel - 1];
    public IEnemy Target { get; set; }
    public DamageMessage GetDamageMessage => new DamageMessage(this.Launcher, this.AttackPower);
    /// <summary>
    /// 攻击力
    /// </summary>
    public float AttackPower => (this.attackPower + (this.centerBuilding.CurrentLevel - 1) * this.attackPowerPromotion) * this.attackPowerPromotionForRound * GameScene.Instance.CurrentRound;
    /// <summary>
    /// 攻击速度
    /// </summary>
    public float AttackSpeed => this.attackSpeed - (this.centerBuilding.CurrentLevel - 1) * this.attackSpeedPromotion;
    /// <summary>
    /// 攻击距离
    /// </summary>
    public float AttackDistance => this.attackDistance + (this.centerBuilding.CurrentLevel - 1) * this.attackDistancePromotion;
    /// <summary>
    /// 弓箭前进速度
    /// </summary>
    public float ArrowSpeed => this.arrowSpeed + (this.centerBuilding.CurrentLevel - 1) * this.arrowSpeedPromotion;


    public void Attack()
    {
        if (this.centerBuilding.IsDie) return;
        //var hits = Physics.SphereCastAll(this.transform.position.SetValue(null, 0f, null), this.AttackDistance, Vector3.up, 100f, LayerMask.GetMask("Enemy"));
        var hits = Physics.OverlapCapsule(this.Launcher.position.SetValue(y: -1000f), this.transform.position.SetValue(y: 1000f), this.AttackDistance, EnemyFactory.ENEMY_LAYER_MASK);
        var enemys = new List<IEnemy>();
        for (var i = 0; i < hits.Length; i++)
        {
            var enemy = hits[i].transform.GetComponent<IEnemy>();
            if (enemy == null || enemy.IsDie) continue;
            enemys.Add(enemy);

            if (enemy == this.Target)
            {
                var bullet = GameObjectFactory.CArrowPoolObject.Take();
                bullet.AfterTakedInitialize(this.Target, this.GetDamageMessage, this.ArrowSpeed, this.GetType());
                return;
            }
        }

        enemys.Sort((a, b) => a.SerialNumber.CompareTo(b.SerialNumber));

        if (enemys.Count > 0)
        {
            this.Target = enemys[0];
            var bullet = GameObjectFactory.CArrowPoolObject.Take();
            bullet.AfterTakedInitialize(this.Target, this.GetDamageMessage, this.ArrowSpeed, this.GetType());
        }
    }

    private void Awake()
    {
        if (!this.centerBuilding) this.centerBuilding = GetComponent<CenterBuilding>();

        if (this.attackSpeedTimer == null) this.attackSpeedTimer = new Timer(this.AttackSpeed);
        else this.attackSpeedTimer.DefiniteTime = this.AttackSpeed;
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
        var circle = new Circle(this.centerBuilding.Effect.transform.position, this.AttackDistance);

        for (var i = 0; i < 20; i++)
        {
            var point1 = circle.CenterPoint + Quaternion.AngleAxis(i * 360f / 20f, Vector3.up) * Vector3.forward;
            var point2 = circle.CenterPoint + Quaternion.AngleAxis((i + 1) * 360f / 20f, Vector3.up) * Vector3.forward;

            Gizmos.DrawLine(circle.ClosestPoint(point1), circle.ClosestPoint(point2));
        }
    }
#endif
}
