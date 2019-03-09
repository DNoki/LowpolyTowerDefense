using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// 计时器
/// </summary>
[System.Serializable]
public class Timer
{
    /// <summary>
    /// 定时（秒）
    /// </summary>
    public float DefiniteTime;

    /// <summary>
    /// 经过时间
    /// </summary>
    public float AfterTime { get; set; }
    /// <summary>
    /// 经过百分比
    /// </summary>
    public float AfterPercent => this.DefiniteTime <= 0f ? 1f : this.AfterTime / this.DefiniteTime;

    /// <summary>
    /// 是否经过了定义时间
    /// </summary>
    /// <returns></returns>
    public bool IsReach()
    {
        if (this.DefiniteTime < 0)
            Debug.LogWarning("计时器定义时间小于 0。");
        if (this.AfterTime < this.DefiniteTime)
            return false;
        this.AfterTime -= this.DefiniteTime;
        return true;
    }
    /// <summary>
    /// 更新并判断是否经过了定义时间
    /// </summary>
    /// <param name="deltaTime"></param>
    /// <returns></returns>
    public bool UpdateAndIsReach(float deltaTime)
    {
        UpdateTimer(deltaTime);
        return IsReach();
    }
    /// <summary>
    /// 更新计时器
    /// </summary>
    /// <param name="deltaTime"></param>
    public void UpdateTimer(float deltaTime)
    {
        this.AfterTime += deltaTime;
    }
    public void Reset() => this.AfterTime = 0f;

    public Timer(float definiteTime)
    {
        this.DefiniteTime = definiteTime;
        this.AfterTime = 0f;
    }
}

