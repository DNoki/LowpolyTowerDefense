using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 怪物工厂
/// </summary>
public class EnemyFactory : MonoBehaviour
{
    /// <summary>
    /// 敌人遮罩层
    /// </summary>
    public static int ENEMY_LAYER_MASK = 1 << 10;

    [SerializeField] private EChomper eChomperPrefab = null;
    [SerializeField] private EBakudan eBakudanPrefab = null;
    [SerializeField] private EDodo eDodoPrefab = null;
    [SerializeField] private EBarbarian eBarbarianPrefab = null;
    [SerializeField] private ENagaGuard eNagaGuardPrefab = null;
    [SerializeField] private EButcher eButcherPrefab = null;
    [SerializeField] private EDanko eDankoPrefab = null;

    #region EnemyPools
    public static EChomperPool EChomperPoolObject { get; private set; }
    public class EChomperPool : ObjectPool<EChomper>
    {
        private EChomper prefab = null;
        protected override EChomper CreateObject() => Instantiate(this.prefab);
        protected override void DestroyObject(EChomper obj) => Destroy(obj.gameObject);
        public EChomperPool(EChomper prefab) : base(true, 10000) { this.prefab = prefab; }
    }
    public static EBakudanPool EBakudanPoolObject { get; private set; }
    public class EBakudanPool : ObjectPool<EBakudan>
    {
        private EBakudan prefab = null;
        protected override EBakudan CreateObject() => Instantiate(this.prefab);
        protected override void DestroyObject(EBakudan obj) => Destroy(obj.gameObject);
        public EBakudanPool(EBakudan prefab) : base(true, 10000) { this.prefab = prefab; }
    }
    public static EDodoPool EDodoPoolObject { get; private set; }
    public class EDodoPool : ObjectPool<EDodo>
    {
        private EDodo prefab = null;
        protected override EDodo CreateObject() => Instantiate(this.prefab);
        protected override void DestroyObject(EDodo obj) => Destroy(obj.gameObject);
        public EDodoPool(EDodo prefab) : base(true, 10000) { this.prefab = prefab; }
    }
    public static EBarbarianPool EBarbarianPoolObject { get; private set; }
    public class EBarbarianPool : ObjectPool<EBarbarian>
    {
        private EBarbarian prefab = null;
        protected override EBarbarian CreateObject() => Instantiate(this.prefab);
        protected override void DestroyObject(EBarbarian obj) => Destroy(obj.gameObject);
        public EBarbarianPool(EBarbarian prefab) : base(true, 10000) { this.prefab = prefab; }
    }
    public static ENagaGuardPool ENagaGuardPoolObject { get; private set; }
    public class ENagaGuardPool : ObjectPool<ENagaGuard>
    {
        private ENagaGuard prefab = null;
        protected override ENagaGuard CreateObject() => Instantiate(this.prefab);
        protected override void DestroyObject(ENagaGuard obj) => Destroy(obj.gameObject);
        public ENagaGuardPool(ENagaGuard prefab) : base(true, 10000) { this.prefab = prefab; }
    }
    public static EButcherPool EButcherPoolObject { get; private set; }
    public class EButcherPool : ObjectPool<EButcher>
    {
        private EButcher prefab = null;
        protected override EButcher CreateObject() => Instantiate(this.prefab);
        protected override void DestroyObject(EButcher obj) => Destroy(obj.gameObject);
        public EButcherPool(EButcher prefab) : base(true, 10000) { this.prefab = prefab; }
    }
    public static EDankoPool EDankoPoolObject { get; private set; }
    public class EDankoPool : ObjectPool<EDanko>
    {
        private EDanko prefab = null;
        protected override EDanko CreateObject() => Instantiate(this.prefab);
        protected override void DestroyObject(EDanko obj) => Destroy(obj.gameObject);
        public EDankoPool(EDanko prefab) : base(true, 10000) { this.prefab = prefab; }
    }
    #endregion


    public IEnemy TakeEnemyForType(IEnemy enemyType)
    {
        if (enemyType is EChomper)
            return EChomperPoolObject.Take();
        if (enemyType is EBakudan)
            return EBakudanPoolObject.Take();
        if (enemyType is EDodo)
            return EDodoPoolObject.Take();
        if (enemyType is EBarbarian)
            return EBarbarianPoolObject.Take();
        if (enemyType is ENagaGuard)
            return ENagaGuardPoolObject.Take();
        if (enemyType is EButcher)
            return EButcherPoolObject.Take();
        if (enemyType is EDanko)
            return EDankoPoolObject.Take();
        return null;
    }
    public Enemy TakeEnemyForType(string typeName)
    {
        if (typeName == "Bakudan")
            return this.eBakudanPrefab;
        if (typeName == "Chomper")
            return this.eChomperPrefab;
        if (typeName == "Dodo")
            return this.eDodoPrefab;
        if (typeName == "Barbarian")
            return this.eBarbarianPrefab;
        if (typeName == "NagaGuard")
            return this.eNagaGuardPrefab;
        if (typeName == "Butcher")
            return this.eButcherPrefab;
        if (typeName == "Danko")
            return this.eDankoPrefab;
        Debug.LogError("TakeEnemyForType 给定名称不存在于列表中");
        return null;
    }

    public void Initialize()
    {
        ENEMY_LAYER_MASK = LayerMask.GetMask("Enemy");

        if (EChomperPoolObject == null) EChomperPoolObject = new EChomperPool(this.eChomperPrefab);
        if (EBakudanPoolObject == null) EBakudanPoolObject = new EBakudanPool(this.eBakudanPrefab);
        if (EDodoPoolObject == null) EDodoPoolObject = new EDodoPool(this.eDodoPrefab);
        if (EBarbarianPoolObject == null) EBarbarianPoolObject = new EBarbarianPool(this.eBarbarianPrefab);
        if (ENagaGuardPoolObject == null) ENagaGuardPoolObject = new ENagaGuardPool(this.eNagaGuardPrefab);
        if (EButcherPoolObject == null) EButcherPoolObject = new EButcherPool(this.eButcherPrefab);
        if (EDankoPoolObject == null) EDankoPoolObject = new EDankoPool(this.eDankoPrefab);

        ClearPool();
    }
    public void ClearPool()
    {
        EChomperPoolObject.ClearAllObj();
        EBakudanPoolObject.ClearAllObj();
        EDodoPoolObject.ClearAllObj();
        EBarbarianPoolObject.ClearAllObj();
        ENagaGuardPoolObject.ClearAllObj();
        EButcherPoolObject.ClearAllObj();
        EDankoPoolObject.ClearAllObj();
    }
}

/// <summary>
/// 敌人
/// </summary>
public interface IEnemy
{
    Transform Transform { get; }
    /// <summary>
    /// 生成序号
    /// </summary>
    int SerialNumber { get; set; }
    //float HP { get; }
    /// <summary>
    /// 是否死亡
    /// </summary>
    bool IsDie { get; }
    /// <summary>
    /// 行为模式
    /// </summary>
    GameMode BehaviourMode { get; set; }
    /// <summary>
    /// Y轴偏移
    /// </summary>
    float OffsetY { get; }

    void UpdatePathAsyn(IEnemyAttackTarget target);
    void ApplyDamage(DamageMessage data);
    void AddBuff(IBuff buff);
    IEnemy AfterTakedInitialize(IEnemy enemy);
    IEnemy AfterTakedInitialize(EnemyGenerator generator);
}

/// <summary>
/// 攻击目标
/// </summary>
public interface IEnemyAttackTarget
{
    Transform Transform { get; }
    Vector2Int CenterPosition { get; }
    float HP { get; }
    bool IsDie { get; }

    /// <summary>
    /// 计算距离攻击目标的距离
    /// </summary>
    /// <param name="position">敌人位置</param>
    /// <returns></returns>
    float CalculateDistance(Vector3 position);
    void ApplyDamage(DamageMessage data);
}

public struct DamageMessage
{
    public Transform Damager;
    public float Amount;
    //public Vector3 Direction;
    //public Vector3 DamageSource;

    public DamageMessage(Transform damager, float amount)
    {
        this.Damager = damager;
        this.Amount = amount;
    }
}