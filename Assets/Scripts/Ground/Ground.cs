using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 地面
/// </summary>
public class Ground : MonoBehaviour
{
    [SerializeField] private FieldArea fieldArea = null;

    public Dictionary<Vector2Int, GroundBlock> Map { get; set; } = new Dictionary<Vector2Int, GroundBlock>();
    /// <summary>
    /// 绘制领域
    /// </summary>
    public FieldArea FieldArea => this.fieldArea == null ? (this.fieldArea = FindObjectOfType<FieldArea>()) : this.fieldArea;


    /// <summary>
    /// 获取地图块信息
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    public GroundBlock GetBlock(Vector2Int pos)
    {
        GroundBlock result = null;
        if (this.Map.TryGetValue(pos, out result))
            return result;


        if (this.Map.ContainsKey(pos)) return this.Map[pos];
        result = new GroundBlock(pos);
        this.Map.Add(pos, result);
        return result;
    }
    /// <summary>
    /// 获取地图区域块信息
    /// </summary>
    /// <param name="mapRect"></param>
    /// <returns></returns>
    public List<GroundBlock> GetBlocks(RectInt mapRect)
    {
        var pos = mapRect.position;
        var blocks = new List<GroundBlock>();
        for (var y = 0; y < mapRect.height; y++)
            for (var x = 0; x < mapRect.width; x++)
                blocks.Add(GetBlock(pos + new Vector2Int(x, y)));
        return blocks;
    }
    /// <summary>
    /// 获取建筑领域
    /// </summary>
    /// <param name="building"></param>
    /// <returns></returns>
    public List<GroundBlock> GetImpactAreas(IBuilding building)
    {
        var centerPos = building.CenterPosition;
        var radius = building.ImpactAreasRadius;

        var rect = new RectInt(centerPos.x - radius, centerPos.y - radius, 2 * radius, 2 * radius);
        var result = new List<GroundBlock>();

        for (var y = 0; y < rect.height; y++)
        {
            for (var x = 0; x < rect.width; x++)
            {
                var xy = new Vector2Int(x, y);
                if ((xy - new Vector2(radius, radius)).magnitude <= radius)
                    result.Add(GetBlock(rect.position + xy));
            }
        }
        return result;
    }
    /// <summary>
    /// 注册道路
    /// </summary>
    /// <param name="area"></param>
    public void RegistRoadGrounds(Vector4 area)
    {
        for (var y = 0; y < area.w; y++)
        {
            for (var x = 0; x < area.z; x++)
            {
                var pos = new Vector2Int((int)area.x + x, (int)area.y + y);
                var block = GetBlock(pos);
                block.State = GroundStateType.ROAD;
            }
        }
    }
    /// <summary>
    /// 注册建筑占用区域块
    /// </summary>
    /// <param name="mapRect"></param>
    /// <param name="buildingRect"></param>
    /// <param name="owner"></param>
    /// <returns></returns>
    public List<GroundBlock> RegistGrounds(IBuilding building)
    {
        if (!CheckCanBuild(building)) return null;
        var buildingAbsoluteRect = building.BuildingRect;
        var blocks = GetBlocks(building.MapRect);
        for (var i = 0; i < blocks.Count; i++)
        {
            if (buildingAbsoluteRect.IsInclude(blocks[i].MapPosition))
                blocks[i].State = GroundStateType.OBSTRUCT;
            else blocks[i].State = GroundStateType.PASSING;
            blocks[i].Owner = building;
        }
        this.FieldArea.AddField(GetImpactAreas(building));
        return blocks;
    }
    /// <summary>
    /// 取消注册建筑占用区域块
    /// </summary>
    /// <param name="building"></param>
    public void UnRegistGrounds(IBuilding building)
    {
        var blocks = GetBlocks(building.MapRect);
        for (var i = 0; i < blocks.Count; i++)
        {
            blocks[i].State = GroundStateType.PASSING;
            blocks[i].Owner = null;
        }
        //this.FieldArea.RemoveField(GetImpactAreas(building));
    }
    /// <summary>
    /// 检查是否可以建造
    /// </summary>
    /// <param name="building"></param>
    /// <returns></returns>
    public bool CheckCanBuild(IBuilding building)
    {
        var mapRect = GetBlocks(building.MapRect);
        var buildingRect = GetBlocks(building.BuildingRect);
        for (var i = 0; i < mapRect.Count; i++)
        {
            // 检查每个要注册的区域块是否已被占用
            if (mapRect[i].Owner != null && mapRect[i].Owner != building)
                return false;
            // 检查是否为道路
            if (mapRect[i].State == GroundStateType.ROAD)
                return false;
        }

        if (GameScene.Instance.GameMode == GameMode.FREE)
            // 检查每个建筑区域是否位于领域内
            foreach (var block in buildingRect)
                if (!this.FieldArea.Field.ContainsKey(block.MapPosition))
                    return false;

        return true;
    }

    /// <summary>
    /// 获取建筑中心向外指定方向的最近格子
    /// </summary>
    /// <param name="building">建筑</param>
    /// <param name="direction">中心向外发射方向</param>
    /// <returns></returns>
    public static Vector2Int GetBuildingProximateMapPos(IBuilding building, Vector3 direction)
    {
        direction.y = 0;
        direction = direction.normalized;
        var center = building.WorldCenterPosition;
        var buildingRect = building.BuildingRect;

        var i = 0;
        while (true)
        {
            var pos = GroundBlock.WorldPositionToMapPos(center + direction * GroundBlock.UNIT * i++);
            if (!buildingRect.IsInclude(pos)) return pos;
        }
    }
}

