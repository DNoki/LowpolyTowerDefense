using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 圆
/// </summary>
[System.Serializable]
public struct Circle
{
    /// <summary>
    /// 圆心
    /// </summary>
    public Vector3 CenterPoint;
    /// <summary>
    /// 半径
    /// </summary>
    public float Radius;

    /// <summary>
    /// 圆的面积
    /// </summary>
    public float Acreage => Mathf.PI * this.Radius * this.Radius;
    /// <summary>
    /// 圆的周长
    /// </summary>
    public float Circumference => 2 * Mathf.PI * this.Radius;

    /// <summary>
    /// 返回某一点到此圆的最近一点
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    public Vector3 ClosestPoint(Vector3 position)
    {
        var normal = (position - this.CenterPoint).normalized;
        return this.CenterPoint + (normal * this.Radius);
    }

    public Circle(Vector3 center, float r)
    {
        this.CenterPoint = center;
        this.Radius = r;
    }
}