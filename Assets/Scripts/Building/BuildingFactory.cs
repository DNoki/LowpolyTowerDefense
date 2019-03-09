using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 建筑工厂
/// 建築工場
/// </summary>
public class BuildingFactory : MonoBehaviour
{
    /// <summary>
    /// 建筑等级上限
    /// レベル上限
    /// </summary>
    public const int BUILDING_LEVEL_LIMIT = 3;

    [SerializeField] private ArrowTower arrowTowerPrefab = null;
    [SerializeField] private ArtilleryTower artilleryTowerPrefab = null;
    [SerializeField] private IceTower iceTowerPrefab = null;
    [SerializeField] private MagicTower magicTowerPrefab = null;
    private ICanBuild previewBuilding = null;

    /// <summary>
    /// 所有建筑管理列表
    /// 建築管理リスト
    /// </summary>
    public List<IBuilding> Buildings { get; private set; } = new List<IBuilding>();
    /// <summary>
    /// 是否正在预览
    /// </summary>
    public bool IsPreviewing => this.previewBuilding != null;


    #region ObjectPool
    /// <summary>
    /// 箭塔池对象
    /// </summary>
    public static ArrowTowerPool ArrowTowerPoolObject { get; private set; }
    /// <summary>
    /// 箭塔对象池
    /// </summary>
    public class ArrowTowerPool : ObjectPool<ArrowTower>
    {
        protected override ArrowTower CreateObject() => Instantiate(GameScene.Instance.BuildingFactory.arrowTowerPrefab);
        protected override void DestroyObject(ArrowTower obj) => Destroy(obj.gameObject);

        public ArrowTowerPool() : base(true, 10000) { }
    }
    public static ArtilleryTowerPool ArtilleryTowerPoolObject { get; private set; }
    public class ArtilleryTowerPool : ObjectPool<ArtilleryTower>
    {
        protected override ArtilleryTower CreateObject() => Instantiate(GameScene.Instance.BuildingFactory.artilleryTowerPrefab);
        protected override void DestroyObject(ArtilleryTower obj) => Destroy(obj.gameObject);

        public ArtilleryTowerPool() : base(true, 10000) { }
    }
    /// <summary>
    /// 冰塔池对象
    /// </summary>
    public static IceTowerPool IceTowerPoolObject { get; private set; }
    public class IceTowerPool : ObjectPool<IceTower>
    {
        protected override IceTower CreateObject() => Instantiate(GameScene.Instance.BuildingFactory.iceTowerPrefab);
        protected override void DestroyObject(IceTower obj) => Destroy(obj.gameObject);

        public IceTowerPool() : base(true, 10000) { }
    }
    /// <summary>
    /// 魔法塔池对象
    /// </summary>
    public static MagicTowerPool MagicTowerPoolObject { get; private set; }
    public class MagicTowerPool : ObjectPool<MagicTower>
    {
        protected override MagicTower CreateObject() => Instantiate(GameScene.Instance.BuildingFactory.magicTowerPrefab);
        protected override void DestroyObject(MagicTower obj) => Destroy(obj.gameObject);

        public MagicTowerPool() : base(true, 10000) { }
    }
    #endregion


    public void CreatePreviewBuilding(string name)
    {
        AudioManager.Instance.Play("Click1");
        ResetPreview();
        ICanBuild tempBuilding = null;
        if (name == nameof(ArrowTower))
            tempBuilding = ArrowTowerPoolObject.Take();
        else if (name == nameof(ArtilleryTower))
            tempBuilding = ArtilleryTowerPoolObject.Take();
        else if (name == nameof(IceTower))
            tempBuilding = IceTowerPoolObject.Take();
        else if (name == nameof(MagicTower))
            tempBuilding = MagicTowerPoolObject.Take();

        // 检查金币是否可以购买
        if (GameScene.Instance.Money >= tempBuilding.ConstructionCost)
        {
            GameScene.Instance.Money -= tempBuilding.ConstructionCost;
            this.previewBuilding = tempBuilding;
            GameScene.Instance.Ground.FieldArea.SetFieldDisplayEnable(true);
            SetBuildingsObstructAreasEnable(true);
        }
        else
        {
            GameScene.Instance.UIManager.Money.TipsMonyeInsufficientCantBuilding();
            ReleaseBuilding(tempBuilding);
        }
    }

    /// <summary>
    /// 添加建筑到管理列表
    /// 建築管理リストに入れる
    /// </summary>
    /// <param name="building"></param>
    public void AddBuilding(IBuilding building)
    {
        if (!this.Buildings.Contains(building))
        {
            this.Buildings.Add(building);
            if (building is ICanBuild)
                (building as ICanBuild).BuildState = BuildState.BUILDING;
            GameScene.Instance.Ground.RegistGrounds(building);
            if (GameScene.Instance.GameMode == GameMode.FREE)
                (GameScene.Instance as GameSceneFree).UpdataGenerateCircle();// 更新怪物生成范围
        }

    }
    /// <summary>
    /// 从管理列表中移除建筑
    /// 建築管理リストから抜く
    /// </summary>
    /// <param name="building"></param>
    public void RemoveBuilding(IBuilding building)
    {
        if (this.Buildings.Contains(building))
        {
            this.Buildings.Remove(building);
            GameScene.Instance.Ground.UnRegistGrounds(building);
        }
    }
    /// <summary>
    /// 设置所有建筑显示不可建造区域
    /// </summary>
    /// <param name="value"></param>
    public void SetBuildingsObstructAreasEnable(bool value)
    {
        for (var i = 0; i < this.Buildings.Count; i++)
        {
            this.Buildings[i].Effect.SetObstructArea(value);
            this.Buildings[i].Effect.SetAttackRange(value);
        }
    }
    /// <summary>
    /// 重置预览建筑
    /// </summary>
    /// <param name="isRelease">是否释放(如果是则放回池并返还金币)</param>
    public void ResetPreview()
    {
        if (this.previewBuilding == null) return;

        this.previewBuilding.Effect.SetObstructArea(false);
        this.previewBuilding.Effect.SetDeployArea(false, false);
        this.previewBuilding.Effect.SetAttackRange(false);

        if (this.previewBuilding.BuildState == BuildState.PREVIEW)
        {
            // 返还金币
            GameScene.Instance.Money += this.previewBuilding.ConstructionCost;
            ReleaseBuilding(this.previewBuilding);
        }
        this.previewBuilding = null;
    }

    /// <summary>
    /// 按类型释放建筑
    /// </summary>
    /// <param name="building"></param>
    public void ReleaseBuilding(ICanBuild building)
    {
        if (building is ArrowTower)
            ArrowTowerPoolObject.Release(building as ArrowTower);
        else if (building is ArtilleryTower)
            ArtilleryTowerPoolObject.Release(building as ArtilleryTower);
        else if (building is IceTower)
            IceTowerPoolObject.Release(building as IceTower);
        else if (building is MagicTower)
            MagicTowerPoolObject.Release(building as MagicTower);
    }

    /// <summary>
    /// 更新预览建筑 并返回是否可以建造
    /// </summary>
    private bool UpdatePreviewBuidling()
    {
        if (this.previewBuilding == null) return false;
        var hit = InputCtrl.MouseGroundPosition;
        if (hit.transform != null)
            this.previewBuilding.SetMapPosition(GroundBlock.WorldPositionToMapPos(hit.point), this.previewBuilding.Transform.position.y);

        return GameScene.Instance.Ground.CheckCanBuild(this.previewBuilding);// 检查是否允许建造
    }

    public void Initialize()
    {
        if (ArrowTowerPoolObject == null) ArrowTowerPoolObject = new ArrowTowerPool();
        if (ArtilleryTowerPoolObject == null) ArtilleryTowerPoolObject = new ArtilleryTowerPool();
        if (IceTowerPoolObject == null) IceTowerPoolObject = new IceTowerPool();
        if (MagicTowerPoolObject == null) MagicTowerPoolObject = new MagicTowerPool();

        ClearPool();
    }
    public void ClearPool()
    {
        ArrowTowerPoolObject.ClearAllObj();
        ArtilleryTowerPoolObject.ClearAllObj();
        IceTowerPoolObject.ClearAllObj();
        MagicTowerPoolObject.ClearAllObj();
    }


    private void Update()
    {
        if (this.previewBuilding != null)
        {
            var canBuild = UpdatePreviewBuidling();
            var mouseOnUI = GameScene.Instance.UIManager.BuildingList.CheckMouseOnBuildingListUI();
            this.previewBuilding.Effect.SetDeployArea(true, canBuild);
            if (InputCtrl.IsRightMouseDown)
            {
                // 取消预览建筑并返还金币
                GameScene.Instance.Ground.FieldArea.SetFieldDisplayEnable(false);
                SetBuildingsObstructAreasEnable(false);

                ResetPreview();
            }
            else if (InputCtrl.IsLeftMouseDown && canBuild && !mouseOnUI)
            {
                GameScene.Instance.Ground.FieldArea.SetFieldDisplayEnable(false);
                SetBuildingsObstructAreasEnable(false);

                AudioManager.Instance.Play("Building");
                AddBuilding(this.previewBuilding);
                ResetPreview();
            }
        }
    }

#warning tiaoshiyong hanshu 调试用函数　デバッグよう
    public static void DrawRange(IBuilding building)
    {
        var rect = building.BuildingRect;
        var lands = GameScene.Instance.Ground.GetBlocks(building.MapRect);
        foreach (var block in lands)
        {
            if (rect.IsInclude(block.MapPosition))
                Gizmos.color = Color.red;
            else Gizmos.color = Color.green;
            var range = GroundBlock.MapPositionToRect(block.MapPosition);
            Gizmos.DrawLine(new Vector3(range.x, 0, range.y), new Vector3(range.xMax, 0, range.yMax));
        }
        Gizmos.color = Color.blue;
        var centerRange = GroundBlock.MapPositionToRect(building.CenterPosition);
        Gizmos.DrawLine(new Vector3(centerRange.xMin, 0, centerRange.yMax), new Vector3(centerRange.xMax, 0, centerRange.yMin));

        Gizmos.color = Color.yellow;
        var worldCenterPos = building.WorldCenterPosition;
        Gizmos.DrawLine(worldCenterPos + Vector3.left, worldCenterPos + Vector3.right);
        Gizmos.DrawLine(worldCenterPos + Vector3.forward, worldCenterPos + Vector3.back);
    }
}

/// <summary>
/// 建筑物
/// </summary>
public interface IBuilding
{
    Transform Transform { get; }
    /// <summary>
    /// 建筑占用地图区域
    /// </summary>
    RectInt MapRect { get; }
    /// <summary>
    /// 建筑占用相对区域
    /// </summary>
    RectInt BuildingRelativeRect { get; }
    /// <summary>
    /// 建筑占用绝对区域
    /// </summary>
    RectInt BuildingRect { get; }
    /// <summary>
    /// 建筑中心相对位置
    /// </summary>
    Vector2Int CenterRelativePosition { get; }
    /// <summary>
    /// 建筑中心绝对位置
    /// </summary>
    Vector2Int CenterPosition { get; }
    /// <summary>
    /// 建筑世界中心绝对位置
    /// </summary>
    Vector3 WorldCenterPosition { get; }
    /// <summary>
    /// 影响领域半径
    /// </summary>
    int ImpactAreasRadius { get; }
    /// <summary>
    /// 建筑效果
    /// </summary>
    BuildingCommonEffect Effect { get; }
    /// <summary>
    /// 当前等级
    /// </summary>
    int CurrentLevel { get; }

    /// <summary>
    /// 升级
    /// </summary>
    void Upgrade();
}

/// <summary>
/// 由玩家建造的建筑物
/// </summary>
public interface ICanBuild : IBuilding
{
    /// <summary>
    /// 建造状态
    /// </summary>
    BuildState BuildState { get; set; }
    /// <summary>
    /// 建造费用
    /// </summary>
    int ConstructionCost { get; }
    /// <summary>
    /// 设置地图位置
    /// </summary>
    /// <param name="centerPos"></param>
    /// <param name="height"></param>
    void SetMapPosition(Vector2Int centerPos, float height);
    /// <summary>
    /// 卖出
    /// </summary>
    void Sell();
}

/// <summary>
/// 建造状态
/// </summary>
public enum BuildState
{
    /// <summary>
    /// 预览中
    /// </summary>
    PREVIEW,
    /// <summary>
    /// 已建造
    /// </summary>
    BUILDING,
}

public interface IBuildingAttackSystem
{
    float AttackDistance { get; }
}