using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 建築のUI
/// </summary>
public class BuildingItemCanvas : MonoBehaviour
{
    public RectTransform RectTransform = null;
    public Vector2 PosOffset = Vector3.zero;
    public RectTransform UpgradeObj = null;
    public RectTransform SellObj = null;

    public Transform GetCameraTransform => Camera.main.transform;
    private IBuilding building = null;

    public static bool IsMouseEnter { get; set; } = false;

    /// <summary>
    /// 设置启用或禁用
    /// </summary>
    /// <param name="enable"></param>
    public void SetEnabled(bool enable)
    {
        this.gameObject.SetActive(enable);
        if (enable) Update();
    }

    public void MouseEnter() => IsMouseEnter = true;
    public void MouseExit() => IsMouseEnter = false;

    public void Upgrade()
    {
        this.building.Upgrade();
    }
    public void Sell()
    {
        (this.building as ICanBuild).Sell();
    }

    public void ResetItem()
    {
        if (this.building is ICanBuild)
        {
            this.UpgradeObj.gameObject.SetActive(true);
            this.SellObj.gameObject.SetActive(true);
        }
    }
    /// <summary>
    /// 隐藏升级按钮
    /// </summary>
    public void HideUpgrade()
    {
        this.UpgradeObj.gameObject.SetActive(false);
        MouseExit();
    }

    private void Awake()
    {
        if (this.RectTransform == null) this.RectTransform = this.GetComponent<RectTransform>();
        if (this.building == null) this.building = this.transform.root.GetComponentInChildren<IBuilding>();
        if (this.UpgradeObj == null) this.UpgradeObj = this.transform.Find("Upgrade").GetComponent<RectTransform>();
        if (this.SellObj == null && this.building is ICanBuild) this.SellObj = this.transform.Find("Sell").GetComponent<RectTransform>();
    }
    private void Update()
    {
        var direction = Vector3.Cross(this.GetCameraTransform.position - this.building.WorldCenterPosition, Vector3.up).normalized;
        direction *= this.PosOffset.x;
        var pos = direction + Vector3.up * this.PosOffset.y;
        this.RectTransform.position = this.building.WorldCenterPosition + pos;
        this.RectTransform.rotation = Quaternion.LookRotation(this.GetCameraTransform.forward, this.GetCameraTransform.up);
    }
}
