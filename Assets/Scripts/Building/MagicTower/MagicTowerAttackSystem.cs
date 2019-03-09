using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 魔法塔攻撃システム
/// </summary>
[RequireComponent(typeof(MagicTower))]
public class MagicTowerAttackSystem : MonoBehaviour, IBuildingAttackSystem
{
    [SerializeField] private MagicTower tower = null;
    [SerializeField] private float attackPower = 1f;
    /// <summary>
    /// 攻击距离
    /// </summary>
    [SerializeField] private float attackDistance = 5f;
    /// <summary>
    /// 最大攻击数量
    /// </summary>
    [SerializeField] private int maxAttackQuantity = 3;
    [SerializeField] private Transform[] launchers = null;

    [SerializeField, Tooltip("攻击力提升")] private float attackPowerPromotion = 2f;
    [SerializeField, Tooltip("攻击距离提升")] private float attackDistancePromotion = 5f;
    [SerializeField, Tooltip("最大攻击数量提升")] private int maxQuantityPromotion = 1;

    /// <summary>
    /// 魔法攻击线列表
    /// </summary>
    private List<MagicLine> lineList = new List<MagicLine>();

    public Transform Launcher => this.launchers[this.tower.CurrentLevel - 1];
    /// <summary>
    /// 获取伤害信息
    /// </summary>
    public DamageMessage GetDamageMessage => new DamageMessage(this.Launcher, this.AttackPower);
    public float AttackPower => this.attackPower + (this.tower.CurrentLevel - 1) * this.attackPowerPromotion;
    /// <summary>
    /// 攻击距离
    /// </summary>
    public float AttackDistance => this.attackDistance + (this.tower.CurrentLevel - 1) * this.attackDistancePromotion;
    public int MaxAttackQuantity => this.maxAttackQuantity + (this.tower.CurrentLevel - 1) * this.maxQuantityPromotion;


    public void Attack()
    {
        if (this.tower.BuildState == BuildState.PREVIEW) return;
        var hits = Physics.OverlapCapsule(this.transform.position.SetValue(y: -1000f), this.transform.position.SetValue(y: 1000f), this.AttackDistance, EnemyFactory.ENEMY_LAYER_MASK);

        // 添加所有能攻击到的敌人
        var enemys = new List<IEnemy>();
        for (var i = 0; i < hits.Length; i++)
        {
            var enemy = hits[i].transform.GetComponent<IEnemy>();
            if (enemy == null || enemy.IsDie) continue;
            enemys.Add(enemy);
        }

        for (var i = 0; i < this.lineList.Count; i++)
        {
            if (!enemys.Contains(this.lineList[this.lineList.Count - 1 - i].Target) || this.lineList[this.lineList.Count - 1 - i].Target.IsDie)
            {
                // 魔法线的目标已经脱离攻击范围 或 目标已死亡
                GameObjectFactory.MagicLinePoolObject.Release(this.lineList[this.lineList.Count - 1 - i]);
                this.lineList.RemoveAt(this.lineList.Count - 1 - i);
            }
        }

        // 添加目标到魔法线
        var RemainQuantity = this.MaxAttackQuantity - this.lineList.Count;// 剩余位置
        if (enemys.Count > 0 && RemainQuantity > 0)
        {
            enemys.Sort((a, b) => a.SerialNumber.CompareTo(b.SerialNumber));
            for (var i = 0; i < RemainQuantity; i++)
            {
                if (i >= enemys.Count) break;
                if (this.lineList.FindIndex(l => l.Target.Equals(enemys[i])) >= 0)
                {
                    RemainQuantity++;
                    continue;
                }
                var line = GameObjectFactory.MagicLinePoolObject.Take();
                line.AfterTakedInitialize(enemys[i], this.GetDamageMessage);
                this.lineList.Add(line);
            }
        }
    }
    /// <summary>
    /// 清空魔法攻击线
    /// </summary>
    public void ClearLine()
    {
        for (var i = 0; i < this.lineList.Count; i++)
            GameObjectFactory.MagicLinePoolObject.Release(this.lineList[i]);
        this.lineList.Clear();
    }

    private void Awake()
    {
        if (!this.tower) this.tower = GetComponent<MagicTower>();

        if (this.lineList == null) this.lineList = new List<MagicLine>();
        else ClearLine();
    }
    private void Update()
    {
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
