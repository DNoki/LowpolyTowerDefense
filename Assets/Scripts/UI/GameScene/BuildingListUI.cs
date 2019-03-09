using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildingListUI : MonoBehaviour
{
    public Button ArrowTower = null;
    public Button ArtilleryTower = null;
    public MessageText ArtilleryTowerMessage = null;
    public Button IceTower = null;
    public MessageText IceTowerMessage = null;
    public Button MagicTower = null;
    public MessageText MagicTowerMessage = null;

    public void PreviewArrowTower()
    {
        GameScene.Instance.BuildingFactory.CreatePreviewBuilding(nameof(ArrowTower));
    }
    public void PreViewArtilleryTower()
    {
        GameScene.Instance.BuildingFactory.CreatePreviewBuilding(nameof(ArtilleryTower));
    }
    public void PreviewIceTower()
    {
        GameScene.Instance.BuildingFactory.CreatePreviewBuilding(nameof(IceTower));
    }
    public void PreviewMagicTower()
    {
        GameScene.Instance.BuildingFactory.CreatePreviewBuilding(nameof(MagicTower));
    }

    /// <summary>
    /// 检查鼠标是否位于该UI界面上
    /// </summary>
    /// <returns></returns>
    public bool CheckMouseOnBuildingListUI()
    {
        var uiManager = GameScene.Instance.UIManager;
        var ray = uiManager.UiCamera.ScreenPointToRay(InputCtrl.MousePosition);
        var hits = Physics2D.GetRayIntersectionAll(ray, float.MaxValue, UIManager.UI_LAYER_MASK);
        if (hits.Length <= 0) return false;

        for (var i = 0; i < hits.Length; i++)
            if (hits[i].transform != null && hits[i].transform.name == this.transform.name)
                return true;

        return false;
    }
    /// <summary>
    /// 当主城升级时解锁按钮
    /// </summary>
    public void CenterBuildingUpgrade()
    {
        if (GameScene.Instance.CenterBuilding.CurrentLevel == 2)
        {
            this.ArtilleryTower.interactable = true;
            this.ArtilleryTowerMessage.Text = $"砲塔\r\n\r\n建造コスト：300";
            this.IceTower.interactable = true;
            this.IceTowerMessage.Text = $"氷塔\r\n\r\n建造コスト：300";
        }
        else if (GameScene.Instance.CenterBuilding.CurrentLevel == 3)
        {
            this.MagicTower.interactable = true;
            this.MagicTowerMessage.Text = $"魔法塔\r\n\r\n建造コスト：350";
        }
    }
}

