using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// 防御タワーのベースクラス
/// </summary>
public abstract class Tower : MonoBehaviour, IBuilding, ICanBuild, ISelectableObject
{
    [SerializeField] private IBuildingAttackSystem attackSystem = null;
    [SerializeField] private BuildingItemCanvas itemCanvas = null;
    /// <summary>
    /// 地图区域
    /// </summary>
    [SerializeField] private RectInt mapRect = new RectInt(Vector2Int.zero, Vector2Int.one);
    /// <summary>
    /// 建筑相对区域
    /// </summary>
    [SerializeField] private RectInt buildingRelativeRect = new RectInt(Vector2Int.zero, Vector2Int.one);
    /// <summary>
    /// 相对中心位置
    /// </summary>
    [SerializeField] private Vector2Int centerRelativePosition = Vector2Int.one;
    /// <summary>
    /// 相对世界中心位置
    /// </summary>
    [SerializeField] private Vector3 worldCenterRelativePosition = Vector3.zero;
    /// <summary>
    /// 影响区域范围半径
    /// </summary>
    [SerializeField] private int impactAreasRadius = 1;
    /// <summary>
    /// 地图区域显示效果
    /// </summary>
    [SerializeField] private BuildingCommonEffect mapRectEffect = null;
    /// <summary>
    /// 建造费用
    /// </summary>
    [SerializeField] private int constructionCost = 0;
    /// <summary>
    /// 升级所需金币
    /// </summary>
    [SerializeField] protected int[] upgradeRequired = new int[2];

    public Transform Transform => this.transform;
    public bool IsSelected { get; set; } = false;
    public bool IsReleased { get; set; }
    /// <summary>
    /// 地图区域
    /// </summary>
    public RectInt MapRect => this.mapRect;
    /// <summary>
    /// 建筑区域（相对于地图区域）
    /// </summary>
    public RectInt BuildingRelativeRect => this.buildingRelativeRect;
    /// <summary>
    /// 建筑占用世界区域
    /// </summary>
    public RectInt BuildingRect => new RectInt(this.MapRect.position + this.BuildingRelativeRect.position, this.BuildingRelativeRect.size);
    /// <summary>
    /// 相对中心位置
    /// </summary>
    public Vector2Int CenterRelativePosition => this.centerRelativePosition;
    /// <summary>
    /// 绝对中心位置
    /// </summary>
    public Vector2Int CenterPosition => this.MapRect.position + this.CenterRelativePosition;
    public Vector3 WorldCenterPosition => this.transform.position + this.worldCenterRelativePosition;
    /// <summary>
    /// 影响区域范围半径
    /// </summary>
    public int ImpactAreasRadius => this.impactAreasRadius;
    public BuildingCommonEffect Effect => this.mapRectEffect;
    /// <summary>
    /// 当前等级
    /// </summary>
    public int CurrentLevel { get; set; } = 1;
    public BuildState BuildState { get; set; }
    public int ConstructionCost => this.constructionCost;
    public IBuildingAttackSystem AttackSystem => this.attackSystem;

    protected void CommonInitialize()
    {
        this.IsReleased = false;
        this.IsSelected = false;
        this.BuildState = BuildState.PREVIEW;
        this.CurrentLevel = 1;
        this.itemCanvas.ResetItem();
        this.Effect.SetLevel();
        this.Effect.SetObstructArea(false);
        this.Effect.SetAttackRange(true);
        this.transform.root.gameObject.SetActive(true);
    }
    protected void CommonRelease()
    {
        this.IsReleased = true;
        this.transform.root.gameObject.SetActive(false);
        GameScene.Instance.BuildingFactory.RemoveBuilding(this);
    }

    public void Selected(bool enable)
    {
        if (this.IsSelected == enable) return;
        this.IsSelected = enable;
        this.itemCanvas.SetEnabled(enable);
        this.Effect.SetSelected(enable);
        this.Effect.SetAttackRange(enable);
    }
    public void PreviewSelect(bool enable)
    {
        if (this.IsReleased || this.IsSelected) return;
        this.Effect.SetPreviewSelect(enable);
    }

    /// <summary>
    /// 设置地图位置
    /// </summary>
    /// <param name="centerPos">建筑区域左下角的绝对位置</param>
    /// <param name="height">高度</param>
    public void SetMapPosition(Vector2Int centerPos, float height)
    {
        this.mapRect = new RectInt(centerPos - this.CenterRelativePosition, this.MapRect.size);
        var pos = GroundBlock.MapPositionToWorldPosition(this.BuildingRect.position, height);
        this.transform.position = pos;
    }

    /// <summary>
    /// 升级
    /// </summary>
    public virtual void Upgrade()
    {
        if (this.CurrentLevel >= BuildingFactory.BUILDING_LEVEL_LIMIT)
            return;

        if (GameScene.Instance.Money >= this.upgradeRequired[this.CurrentLevel - 1])
        {
            GameScene.Instance.Money -= this.upgradeRequired[this.CurrentLevel - 1];
            this.CurrentLevel++;
            AudioManager.Instance.Play("Upgrade1");
            AudioManager.Instance.Play("Upgrade2");

            if (this.CurrentLevel >= BuildingFactory.BUILDING_LEVEL_LIMIT)
                this.itemCanvas.HideUpgrade();

            this.Effect.SetLevel();
            this.Effect.UpdateAttackRange();
            MessageBoxUpdater.ResetCurrentTextBox();
        }
        else
        {
            GameScene.Instance.UIManager.Money.TipsMonyeInsufficientCantUpgrate();
            return;
        }
    }
    /// <summary>
    /// 卖出
    /// </summary>
    public virtual void Sell()
    {
        // 返还金币
        var money = this.ConstructionCost;
        if (this.CurrentLevel > 1) money += this.upgradeRequired[0];
        if (this.CurrentLevel > 2) money += this.upgradeRequired[1];
        money = Mathf.CeilToInt(money * 0.5f);
        GameScene.Instance.Money += money;
        AudioManager.Instance.Play("Sell");

        this.itemCanvas.MouseExit();
        GameScene.Instance.Selection.Clear();

        GameScene.Instance.BuildingFactory.ReleaseBuilding(this);
    }

    private void Awake()
    {
        if (!this.mapRectEffect) this.mapRectEffect = GetComponentInChildren<BuildingCommonEffect>();
        if (this.attackSystem == null) this.attackSystem = GetComponentInChildren<IBuildingAttackSystem>();
        if (!this.itemCanvas) this.itemCanvas = GetComponentInChildren<BuildingItemCanvas>();
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        BuildingFactory.DrawRange(this);
    }
#endif
}