using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 建筑效果
/// </summary>
public class BuildingCommonEffect : MonoBehaviour
{
    [SerializeField] private TowerRenderer[] meshRenderers = null;
    [SerializeField] private Renderer deployAreaRenderer = null;
    [SerializeField] private Renderer obstructRenderer = null;
    [SerializeField] private Transform attackRange = null;
    public ParticleSystem.MinMaxGradient PreviewColor = new ParticleSystem.MinMaxGradient(new Color(0.9952626f, 0f, 1f, 1f), new Color(0.9952626f, 0f, 1f, 1f));
    public ParticleSystem.MinMaxGradient SelectedColor = new ParticleSystem.MinMaxGradient(new Color(0f, 1f, 0.1359537f, 1f), new Color(0f, 1f, 0.1359537f, 1f));

    private IBuilding building = null;
    private IBuildingAttackSystem buildingAttackSystem = null;
    private TowerRenderer currentTowerRenderer = null;


    /// <summary>
    /// 设置（不）可建造区域效果显示
    /// </summary>
    /// <param name="enable">是否显示</param>
    /// <param name="value">是否可以建造</param>
    public void SetDeployArea(bool enable, bool value)
    {
        var alpha = this.deployAreaRenderer.material.color.a;
        if (this.deployAreaRenderer.enabled != enable) this.deployAreaRenderer.enabled = enable;
        if (value) this.deployAreaRenderer.material.color = new Color(0, 1, 0, alpha);
        else this.deployAreaRenderer.material.color = new Color(1, 0, 0, alpha);
    }
    /// <summary>
    /// 设置区域不可建造效果显示
    /// </summary>
    /// <param name="enable"></param>
    public void SetObstructArea(bool enable)
    {
        if (this.obstructRenderer.enabled != enable) this.obstructRenderer.enabled = enable;
    }
    /// <summary>
    /// 设置攻击范围效果显示
    /// </summary>
    /// <param name="enable"></param>
    public void SetAttackRange(bool enable)
    {
        UpdateAttackRange();
        if (this.attackRange.gameObject.activeSelf != enable)
            this.attackRange.gameObject.SetActive(enable);
    }
    /// <summary>
    /// 更新攻击范围显示圈
    /// </summary>
    public void UpdateAttackRange()
    {
        var value = this.buildingAttackSystem.AttackDistance;
        this.attackRange.localScale = new Vector3(value, value, value);
    }

    public void SetLevel()
    {
        for (var i = 0; i < this.meshRenderers.Length; i++)
        {
            this.meshRenderers[i].SetEnable(false);
        }
        this.currentTowerRenderer = this.meshRenderers[this.building.CurrentLevel - 1];
        this.currentTowerRenderer.SetEnable(true);
    }
    /// <summary>
    /// 设置被选择效果
    /// </summary>
    /// <param name="enable"></param>
    public void SetSelected(bool enable)
    {
        if (enable)
            this.currentTowerRenderer.SetColor(this.SelectedColor.colorMin, this.SelectedColor.colorMax);
        else this.currentTowerRenderer.SetColor(Color.white, Color.black);
    }
    /// <summary>
    /// 设置选择预览效果
    /// </summary>
    /// <param name="enable"></param>
    public void SetPreviewSelect(bool enable)
    {
        if (enable)
            this.currentTowerRenderer.SetColor(this.PreviewColor.colorMin, this.PreviewColor.colorMax);
        else this.currentTowerRenderer.SetColor(Color.white, Color.black);
    }

    private void Awake()
    {
        this.building = this.transform.root.GetComponentInChildren<IBuilding>();
        this.buildingAttackSystem = this.transform.root.GetComponentInChildren<IBuildingAttackSystem>();
        if (this.deployAreaRenderer != null) this.deployAreaRenderer.transform.localScale = new Vector3(this.building.MapRect.width, this.building.MapRect.height, this.building.MapRect.height);
        this.obstructRenderer.transform.localScale = new Vector3(this.building.MapRect.width, this.building.MapRect.height, this.building.MapRect.height);
        this.obstructRenderer.material.SetVector("_Scale", new Vector4(1 + (this.building.MapRect.width - 4) * 0.25f, 1 + (this.building.MapRect.height - 4) * 0.25f, 0f, 0f));
        this.currentTowerRenderer = this.meshRenderers[0];
    }
}
