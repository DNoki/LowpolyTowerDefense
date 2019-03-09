using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 中心建筑
/// センター建物
/// </summary>
public class CenterBuilding : MonoBehaviour, IBuilding, IEnemyAttackTarget, ISelectableObject, IMessageBox
{
    [SerializeField] private CenterBuildingAttackSystem attackSystem = null;
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
    /// 血量上限
    /// </summary>
    [SerializeField] private float hpLimit = 200f;
    /// <summary>
    /// 升级所需金币
    /// </summary>
    [SerializeField] private int[] upgradeRequired = new int[2];

    [SerializeField] private UnityEventT1floatT2float onDamage = null;
    [SerializeField] protected UnityEvent onCenterLevelChange = null;


    public bool IsSelected { get; set; } = false;
    public Transform Transform => this.transform;
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
    /// <summary>
    /// 地图区域显示效果
    /// </summary>
    public BuildingCommonEffect Effect => this.mapRectEffect;
    /// <summary>
    /// 当前等级
    /// </summary>
    public int CurrentLevel { get; set; } = 1;
    /// <summary>
    /// 血量上限
    /// </summary>
    public float HPLimit { get { return this.hpLimit; } set { this.hpLimit = value; } }
    /// <summary>
    /// 血量
    /// </summary>
    public float HP { get; set; } = 200f;
    /// <summary>
    /// 是否死亡
    /// </summary>
    public bool IsDie => this.HP <= 0;
    string IMessageBox.GetMessageText
    {
        get
        {
            if (this.CurrentLevel < BuildingFactory.BUILDING_LEVEL_LIMIT)
                return $"センター  Lv：{this.CurrentLevel}  HP：{this.HP}\r\n\r\n攻撃力：{this.attackSystem.AttackPower}\r\n攻撃スピード：{this.attackSystem.AttackSpeed}\r\nアップグレードコスト：{this.upgradeRequired[this.CurrentLevel - 1]}";
            else return $"センター  Lv：{this.CurrentLevel}  HP：{this.HP}\r\n\r\n攻撃力：{this.attackSystem.AttackPower}\r\n攻撃スピード：{this.attackSystem.AttackSpeed}";
        }
    }

    /// <summary>
    /// 升级
    /// </summary>
    public void Upgrade()
    {
        if (this.CurrentLevel >= BuildingFactory.BUILDING_LEVEL_LIMIT)
            return;

        if (GameScene.Instance.Money >= this.upgradeRequired[this.CurrentLevel - 1])
        {
            GameScene.Instance.Money -= this.upgradeRequired[this.CurrentLevel - 1];
            this.CurrentLevel++;
            this.onCenterLevelChange.Invoke();
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

    public float CalculateDistance(Vector3 position)
    {
        var pos = GroundBlock.WorldPositionToMapPos(position);
        var proximatePos = Ground.GetBuildingProximateMapPos(this, position - this.WorldCenterPosition);
        //var distance = (proximatePos - pos).magnitude * GroundBlock.UNIT + GroundBlock.UNIT;
        return Vector2Int.Distance(pos, proximatePos);
    }
    public void ApplyDamage(DamageMessage data)
    {
        if (this.IsDie) return;
        if (!this.IsDie)
        {
            this.HP -= data.Amount;
            this.onDamage.Invoke(this.HP, this.HPLimit);
        }
        if (this.IsDie)
        {
            this.attackSystem.enabled = false;
            GameScene.Instance.GameOver(false);
        }
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
        if (this.IsSelected) return;
        this.Effect.SetPreviewSelect(enable);
    }

    private void Awake()
    {
        if (!this.mapRectEffect) this.mapRectEffect = GetComponentInChildren<BuildingCommonEffect>();
        if (!this.attackSystem) this.attackSystem = GetComponentInChildren<CenterBuildingAttackSystem>();
        if (!this.itemCanvas) this.itemCanvas = GetComponentInChildren<BuildingItemCanvas>();
    }
    private void Start()
    {
        // 注册占用土地
        var buildingRect = this.BuildingRect;
        var blocks = GameScene.Instance.Ground.GetBlocks(this.MapRect);
        foreach (var block in blocks)
        {
            if (buildingRect.IsInclude(block.MapPosition))
                block.State = GroundStateType.OBSTRUCT;
            else block.State = GroundStateType.PASSING;
            block.Owner = this;
        }
        GameScene.Instance.Ground.FieldArea.AddField(GameScene.Instance.Ground.GetImpactAreas(this));
        GameScene.Instance.BuildingFactory.AddBuilding(this);

        this.HP = this.HPLimit;
        this.onDamage.Invoke(this.HP, this.HPLimit);
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        BuildingFactory.DrawRange(this);
    }
#endif 
}
