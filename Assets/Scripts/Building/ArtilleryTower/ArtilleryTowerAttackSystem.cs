using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 砲塔の攻撃システム
/// </summary>
[RequireComponent(typeof(ArtilleryTower))]
public class ArtilleryTowerAttackSystem : MonoBehaviour, IBuildingAttackSystem
{
    [SerializeField] private ArtilleryTower tower = null;
    /// <summary>
    /// 攻击力
    /// 攻撃力
    /// </summary>
    [SerializeField] private float attackPower = 10f;
    /// <summary>
    /// 攻击速度
    /// 攻撃スピード
    /// </summary>
    [SerializeField] private float attackSpeed = 3f;
    /// <summary>
    /// 攻击距离
    /// 攻撃距離
    /// </summary>
    [SerializeField] private float attackDistance = 5f;
    /// <summary>
    /// 炮弹前进速度
    /// 矢の移動スピード
    /// </summary>
    [SerializeField] private float ammunitionSpeed = 10f;
    /// <summary>
    /// 爆炸效果范围
    /// 発射位置と発射角度
    /// </summary>
    [SerializeField] private float explosionRange = 3f;
    [SerializeField] private Transform[] sasaes = null;
    [SerializeField] private Transform[] launchers = null;
    /// <summary>
    /// 旋转速度
    /// 回転スピード
    /// </summary>
    [SerializeField] private float rotateSpeed = 2f;

    [SerializeField, Tooltip("攻击力提升")] private float attackPowerPromotion = 10f;
    [SerializeField, Tooltip("攻速提升")] private float attackSpeedPromotion = 1f;
    [SerializeField, Tooltip("攻击距离提升")] private float attackDistancePromotion = 5f;
    [SerializeField, Tooltip("炮弹前进速度提升")] private float ammunitionSpeedPromotion = 2f;
    [SerializeField, Tooltip("爆炸范围提升")] private float explosionRangePromotion = 1f;

    private Timer attackSpeedTimer = new Timer(3f);

    public Transform Sasae => this.sasaes[this.tower.CurrentLevel - 1];
    public Transform Launcher => this.launchers[this.tower.CurrentLevel - 1];
    public DamageMessage GetDamageMessage => new DamageMessage(this.Launcher, this.AttackPower);
    public float AttackPower => this.attackPower + (this.tower.CurrentLevel - 1) * this.attackPowerPromotion;
    public float AttackSpeed => this.attackSpeed - (this.tower.CurrentLevel - 1) * this.attackSpeedPromotion;
    /// <summary>
    /// 攻击距离
    /// 攻撃距離
    /// </summary>
    public float AttackDistance => this.attackDistance + (this.tower.CurrentLevel - 1) * this.attackDistancePromotion;
    public float AmmunitionSpeed => this.ammunitionSpeed + (this.tower.CurrentLevel - 1) * this.ammunitionSpeedPromotion;
    public float ExplosionRange => this.explosionRange + (this.tower.CurrentLevel - 1) * this.explosionRangePromotion;


    public void Attack()
    {
        var isAttack = false;
        if (this.attackSpeedTimer.UpdateAndIsReach(Time.deltaTime))
            isAttack = true;

        if (this.tower.BuildState == BuildState.PREVIEW) return;
        var hits = Physics.OverlapCapsule(this.transform.position.SetValue(y: -1000f), this.transform.position.SetValue(y: 1000f), this.AttackDistance, EnemyFactory.ENEMY_LAYER_MASK);
        var enemys = new List<IEnemy>();
        for (var i = 0; i < hits.Length; i++)
        {
            var enemy = hits[i].transform.GetComponent<IEnemy>();
            if (enemy == null || enemy.IsDie) continue;
            enemys.Add(enemy);
        }

        enemys.Sort((a, b) => a.SerialNumber.CompareTo(b.SerialNumber));
        if (enemys.Count > 0)
        {
            var target = enemys[0];

            for (var i = 0; i < this.sasaes.Length; i++)
            {
                var sasae = this.sasaes[i];
                var rotation = Quaternion.LookRotation((target.Transform.position - sasae.position).SetValue(y: 0));
                sasae.rotation = Quaternion.Lerp(sasae.rotation, rotation, this.rotateSpeed * Time.deltaTime);
            }

            if (isAttack)
            {
                var ammunition = GameObjectFactory.AmmunitionPoolObject.Take();
                ammunition.AfterTakedInitialize(target.Transform.position, this.GetDamageMessage, this.AmmunitionSpeed, this.ExplosionRange);
            }
        }


    }

    private void Awake()
    {
        if (!this.tower) this.tower = GetComponent<ArtilleryTower>();

        if (this.attackSpeedTimer == null) this.attackSpeedTimer = new Timer(this.AttackSpeed);
        else this.attackSpeedTimer.DefiniteTime = this.AttackSpeed;
        this.attackSpeedTimer.AfterTime = Random.Range(0, this.attackSpeedTimer.DefiniteTime);
    }
    private void Update()
    {
        if (this.attackSpeedTimer.DefiniteTime != this.AttackSpeed)
            this.attackSpeedTimer.DefiniteTime = this.AttackSpeed;
        Attack();
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
