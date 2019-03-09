using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 减速Buff
/// </summary>
public struct SlowDownDebuff : IBuff
{
    /// <summary>
    /// 经过时间
    /// </summary>
    private float afterTime;

    /// <summary>
    /// 减速强度（0~1）
    /// </summary>
    public float DecelerationPower { get; set; }
    /// <summary>
    /// 持续时间
    /// </summary>
    public float Duration { get; set; }
    /// <summary>
    /// 强度衰减曲线
    /// </summary>
    public AnimationCurve Curve { get; set; }
    /// <summary>
    /// Buff效果是否已经结束
    /// </summary>
    public bool IsOver => this.afterTime >= this.Duration;

    /// <summary>
    /// 更新时间并返回影响系数
    /// </summary>
    /// <param name="deltaTime"></param>
    /// <returns></returns>
    public float UpdateAndCalculation(float deltaTime)
    {
        this.afterTime += deltaTime;
        return 1 - (1 - this.DecelerationPower) * this.Curve.Evaluate(this.afterTime / this.Duration);
    }

    public SlowDownDebuff(float decelerationPower, float duration, AnimationCurve curve)
    {
        this.afterTime = 0f;
        this.DecelerationPower = decelerationPower;
        this.Duration = duration;
        this.Curve = curve;
    }
}

public interface IBuff
{
    /// <summary>
    /// Buff效果是否已经结束
    /// </summary>
    bool IsOver { get; }
    /// <summary>
    /// 更新时间并返回影响系数
    /// </summary>
    /// <param name="deltaTime"></param>
    /// <returns></returns>
    float UpdateAndCalculation(float deltaTime);
}