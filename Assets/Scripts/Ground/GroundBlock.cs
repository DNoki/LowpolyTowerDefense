using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 地图块
/// </summary>
public class GroundBlock
{
    /// <summary>
    /// 地图单位块长度
    /// </summary>
    public const int UNIT = 1;

    /// <summary>
    /// 位置坐标
    /// </summary>
    public Vector2Int MapPosition { get; set; }
    /// <summary>
    /// 持有者
    /// </summary>
    public IBuilding Owner { get; set; }
    /// <summary>
    /// 地面状态
    /// </summary>
    public GroundStateType State { get; set; }


    /// <summary>
    /// 地图坐标中心转化为世界位置（地图块中心）
    /// </summary>
    /// <param name="map">地图坐标</param>
    /// <param name="height">世界位置高度</param>
    /// <returns></returns>
    public static Vector3 MapPostioinCenterToWorldPosition(Vector2Int map, float height = 0) => new Vector3(map.x * UNIT + 0.5f * UNIT, height, map.y * UNIT + 0.5f * UNIT);
    /// <summary>
    /// 地图坐标转化为世界位置（地图块左下角）
    /// </summary>
    /// <param name="map">地图坐标</param>
    /// /// <param name="height">世界位置高度</param>
    /// <returns></returns>
    public static Vector3 MapPositionToWorldPosition(Vector2Int map, float height = 0) => new Vector3(map.x * UNIT, height, map.y * UNIT);
    /// <summary>
    /// 世界坐标转化为地图坐标
    /// </summary>
    /// <param name="world"></param>
    /// <returns></returns>
    public static Vector2Int WorldPositionToMapPos(Vector3 world) => new Vector2Int(Mathf.FloorToInt(world.x / UNIT), Mathf.FloorToInt(world.z / UNIT));
    /// <summary>
    /// 地图坐标转化为范围
    /// </summary>
    /// <param name="mapPos">地图坐标</param>
    /// <returns></returns>
    public static Rect MapPositionToRect(Vector2Int mapPos) => new Rect(mapPos.x * UNIT, mapPos.y * UNIT, UNIT, UNIT);


    public GroundBlock(Vector2Int pos, GroundStateType state = GroundStateType.PASSING, IBuilding owner = null)
    {
        this.MapPosition = pos;
        this.State = state;
        this.Owner = owner;
    }
}

/// <summary>
/// 地面状态
/// </summary>
public enum GroundStateType
{
    /// <summary>
    /// 可通过、陆地
    /// </summary>
    PASSING,
    /// <summary>
    /// 不可通过、墙壁或建筑
    /// </summary>
    OBSTRUCT,
    ///// <summary>
    ///// 海洋
    ///// </summary>
    //OCEAN,
    /// <summary>
    /// 道路
    /// </summary>
    ROAD,
}
