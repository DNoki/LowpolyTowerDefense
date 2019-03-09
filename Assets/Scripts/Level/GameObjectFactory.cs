using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectFactory : MonoBehaviour
{
    [SerializeField] private CoinEffect coinEffectPrefab = null;
    [SerializeField] private EnemyDirectionEffect enemyDirectionEffectPrefab = null;
    [SerializeField] private BakuhatuEffect bakuhatuEffectPrefab = null;
    [SerializeField] private Arrow arrowPrefab = null;
    [SerializeField] private ArrowContactEffect arrowTouchEffectPrefab = null;
    [SerializeField] private ArrowContactFlashEffect arrowContactFlashEffectPrefab = null;
    [SerializeField] private Ammunition ammunitionPrefab = null;
    [SerializeField] private AmmunitionSmokeEffect ammunitionSmokeEffectPrefab = null;
    /// <summary>
    /// 中心建筑弓箭预制体
    /// </summary>
    [SerializeField] private Arrow centerBuildingArrowPrefab = null;
    [SerializeField] private IceCircle iceCirclePrefab = null;
    [SerializeField] private SlowDownEffect slowDownEffectPrefab = null;
    [SerializeField] private MagicLine magicLinePrefab = null;

    #region 效果
    public static CoinEffectPool CoinEffectPoolObject { get; private set; }
    public class CoinEffectPool : ObjectPool<CoinEffect>
    {
        private CoinEffect prefab = null;
        protected override CoinEffect CreateObject() => Instantiate(this.prefab);
        protected override void DestroyObject(CoinEffect obj) => Destroy(obj.gameObject);

        public CoinEffectPool(CoinEffect prefab) : base(true, 10000) { this.prefab = prefab; }
    }
    public static EnemyDirectionEffectPool EnemyDirectionEffectPoolObject { get; private set; }
    public class EnemyDirectionEffectPool : ObjectPool<EnemyDirectionEffect>
    {
        private EnemyDirectionEffect prefab = null;
        protected override EnemyDirectionEffect CreateObject() => Instantiate(this.prefab);
        protected override void DestroyObject(EnemyDirectionEffect obj) => Destroy(obj.gameObject);

        public EnemyDirectionEffectPool(EnemyDirectionEffect prefab) : base(true, 10000) { this.prefab = prefab; }
    }
    public static BakuhatuEffectPool BakuhatuEffectPoolObject { get; private set; }
    public class BakuhatuEffectPool : ObjectPool<BakuhatuEffect>
    {
        private BakuhatuEffect prefab = null;
        protected override BakuhatuEffect CreateObject() => Instantiate(this.prefab);
        protected override void DestroyObject(BakuhatuEffect obj) => Destroy(obj.gameObject);

        public BakuhatuEffectPool(BakuhatuEffect prefab) : base(true, 10000) { this.prefab = prefab; }
    }
    #endregion

    #region Arrow
    public static ArrowPool ArrowPoolObject { get; private set; } = null;
    public class ArrowPool : ObjectPool<Arrow>
    {
        private Arrow prefab = null;
        protected override Arrow CreateObject() => Instantiate(this.prefab);
        protected override void DestroyObject(Arrow obj) => Destroy(obj.gameObject);

        public ArrowPool(Arrow prefab) : base(true, 10000) { this.prefab = prefab; }
    }
    public static ArrowContactEffectPool ArrowTouchEffectPoolObject { get; private set; }
    public class ArrowContactEffectPool : ObjectPool<ArrowContactEffect>
    {
        private ArrowContactEffect prefab = null;

        protected override ArrowContactEffect CreateObject() => Instantiate(this.prefab);
        protected override void DestroyObject(ArrowContactEffect obj) => Destroy(obj.gameObject);

        public ArrowContactEffectPool(ArrowContactEffect prefab) : base(true, 10000) { this.prefab = prefab; }
    }
    public static ArrowContactFlashEffectPool ArrowContactFlashEffectPoolObject { get; private set; }
    public class ArrowContactFlashEffectPool : ObjectPool<ArrowContactFlashEffect>
    {
        private ArrowContactFlashEffect prefab = null;

        protected override ArrowContactFlashEffect CreateObject() => Instantiate(this.prefab);
        protected override void DestroyObject(ArrowContactFlashEffect obj) => Destroy(obj.gameObject);

        public ArrowContactFlashEffectPool(ArrowContactFlashEffect prefab) : base(true, 10000) { this.prefab = prefab; }
    }
    /// <summary>
    /// 中心建筑弓箭对象池
    /// </summary>
    public static CArrowPool CArrowPoolObject { get; private set; } = null;
    public class CArrowPool : ObjectPool<Arrow>
    {
        private Arrow prefab = null;
        protected override Arrow CreateObject() => Instantiate(this.prefab);
        protected override void DestroyObject(Arrow obj) => Destroy(obj.gameObject);

        public CArrowPool(Arrow prefab) : base(true, 10000) { this.prefab = prefab; }
    }
    #endregion

    #region Ammunition
    public static AmmunitionPool AmmunitionPoolObject { get; private set; } = null;
    public class AmmunitionPool : ObjectPool<Ammunition>
    {
        private Ammunition prefab = null;
        protected override Ammunition CreateObject() => Instantiate(this.prefab);
        protected override void DestroyObject(Ammunition obj) => Destroy(obj.gameObject);

        public AmmunitionPool(Ammunition prefab) : base(true, 10000) { this.prefab = prefab; }
    }
    public static AmmunitionSmokeEffectPool AmmunitionSmokeEffectPoolObject { get; private set; }
    public class AmmunitionSmokeEffectPool : ObjectPool<AmmunitionSmokeEffect>
    {
        private AmmunitionSmokeEffect prefab = null;

        protected override AmmunitionSmokeEffect CreateObject() => Instantiate(this.prefab);
        protected override void DestroyObject(AmmunitionSmokeEffect obj) => Destroy(obj.gameObject);

        public AmmunitionSmokeEffectPool(AmmunitionSmokeEffect prefab) : base(true, 10000) { this.prefab = prefab; }
    }
    #endregion

    #region Ice
    public static IceCirclePool IceCirclePoolObject { get; private set; } = null;
    public class IceCirclePool : ObjectPool<IceCircle>
    {
        private IceCircle prefab = null;
        protected override IceCircle CreateObject() => Instantiate(this.prefab);
        protected override void DestroyObject(IceCircle obj) => Destroy(obj.gameObject);

        public IceCirclePool(IceCircle prefab) : base(true, 10000) { this.prefab = prefab; }
    }
    /// <summary>
    /// 附着在敌人身上的减速显示特效对象池
    /// </summary>
    public static SlowDownEffectPool SlowDownEffectPoolObject { get; private set; }
    public class SlowDownEffectPool : ObjectPool<SlowDownEffect>
    {
        private SlowDownEffect prefab = null;

        protected override SlowDownEffect CreateObject() => Instantiate(this.prefab);
        protected override void DestroyObject(SlowDownEffect obj) => Destroy(obj.gameObject);

        public SlowDownEffectPool(SlowDownEffect prefab) : base(true, 10000) { this.prefab = prefab; }
    }
    #endregion

    #region Magic
    public static MagicLinePool MagicLinePoolObject { get; private set; }
    public class MagicLinePool : ObjectPool<MagicLine>
    {
        private MagicLine prefab = null;

        protected override MagicLine CreateObject() => Instantiate(this.prefab);
        protected override void DestroyObject(MagicLine obj) => Destroy(obj.gameObject);

        public MagicLinePool(MagicLine prefab) : base(true, 1000) { this.prefab = prefab; }
    }
    #endregion



    public void Initialize()
    {
        if (CoinEffectPoolObject == null) CoinEffectPoolObject = new CoinEffectPool(this.coinEffectPrefab);
        if (EnemyDirectionEffectPoolObject == null) EnemyDirectionEffectPoolObject = new EnemyDirectionEffectPool(this.enemyDirectionEffectPrefab);
        if (BakuhatuEffectPoolObject == null) BakuhatuEffectPoolObject = new BakuhatuEffectPool(this.bakuhatuEffectPrefab);
        if (ArrowPoolObject == null) ArrowPoolObject = new ArrowPool(this.arrowPrefab);
        if (ArrowTouchEffectPoolObject == null) ArrowTouchEffectPoolObject = new ArrowContactEffectPool(this.arrowTouchEffectPrefab);
        if (ArrowContactFlashEffectPoolObject == null) ArrowContactFlashEffectPoolObject = new ArrowContactFlashEffectPool(this.arrowContactFlashEffectPrefab);
        if (AmmunitionPoolObject == null) AmmunitionPoolObject = new AmmunitionPool(this.ammunitionPrefab);
        if (AmmunitionSmokeEffectPoolObject == null) AmmunitionSmokeEffectPoolObject = new AmmunitionSmokeEffectPool(this.ammunitionSmokeEffectPrefab);
        if (CArrowPoolObject == null) CArrowPoolObject = new CArrowPool(this.centerBuildingArrowPrefab);
        if (IceCirclePoolObject == null) IceCirclePoolObject = new IceCirclePool(this.iceCirclePrefab);
        if (SlowDownEffectPoolObject == null) SlowDownEffectPoolObject = new SlowDownEffectPool(this.slowDownEffectPrefab);
        if (MagicLinePoolObject == null) MagicLinePoolObject = new MagicLinePool(this.magicLinePrefab);

        ClearPool();
    }
    public void ClearPool()
    {
        ArrowPoolObject.ClearAllObj();
        CoinEffectPoolObject.ClearAllObj();
        BakuhatuEffectPoolObject.ClearAllObj();
        EnemyDirectionEffectPoolObject.ClearAllObj();
        ArrowTouchEffectPoolObject.ClearAllObj();
        ArrowContactFlashEffectPoolObject.ClearAllObj();
        AmmunitionPoolObject.ClearAllObj();
        AmmunitionSmokeEffectPoolObject.ClearAllObj();
        CArrowPoolObject.ClearAllObj();
        IceCirclePoolObject.ClearAllObj();
        SlowDownEffectPoolObject.ClearAllObj();
        MagicLinePoolObject.ClearAllObj();
    }
}
