using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// 抛物运动
/// 斜方投射
/// </summary>
public struct ProjectileMotion
{
    /// <summary>
    /// 初始位置
    /// </summary>
    public Vector3 InitialPosition { get; set; }
    /// <summary>
    /// 初始速度 V0
    /// </summary>
    public Vector3 InitialVelocity { get; set; }
    /// <summary>
    /// 所受阻力
    /// </summary>
    public Vector3 Drag { get; set; }
    /// <summary>
    /// 阻力加速度
    /// </summary>
    public Vector3 DragA { get; set; }
    /// <summary>
    /// 重力
    /// </summary>
    public Vector3 Gravity { get; set; }

    /// <summary>
    /// 初始发射角 θ
    /// </summary>
    public float InitialLaunchAngle => Vector3.Angle(this.InitialVelocity, Vector3.ProjectOnPlane(this.InitialVelocity, Vector3.up));
    /// <summary>
    /// 抛物运动到达最大高度所需要的时间
    /// </summary>
    public float TimeToReachMaxHeight => Mathf.Max((this.InitialVelocity.y + this.Drag.y) / (-this.Gravity.y + -this.DragA.y), 0f);
    /// <summary>
    /// 最大高度位置
    /// </summary>
    public Vector3 MaxHeightPosition => GetPosition(this.TimeToReachMaxHeight);
    /// <summary>
    /// 抛射后落回地面的位置
    /// </summary>
    public Vector3 MaxDistancePosition => GetPosition(this.TimeToReachMaxHeight * 2);


    /// <summary>
    /// 获取任何时间t的速度
    /// </summary>
    /// <param name="time">经过时间</param>
    /// <returns></returns>
    public Vector3 GetVelocity(float time) => this.InitialVelocity + (this.Gravity * time) + (this.Drag + this.DragA * time);
    /// <summary>
    /// 获取任何时间t的位置
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    public Vector3 GetPosition(float time)
    {
        var accelerationCoefficient = 0.5f * Mathf.Pow(time, 2f);// 加速度系数

        var selfEffect = this.InitialVelocity * time;
        var gravityEffect = this.Gravity * accelerationCoefficient;
        var dragAEffect = (this.Drag * time) + this.DragA * accelerationCoefficient;

        // 位置 = 初始位置 + 位移 = 初始位置 + （自身影响 + 重力影响 + 阻力影响）
        var result = this.InitialPosition + (selfEffect + gravityEffect + dragAEffect);
        return result;
    }

    /// <summary>
    /// 给定初始速度大小，目标位置，求角度（复数解），不考虑重力xz方向分力和阻力
    /// </summary>
    /// <param name="initialPosition">初始位置</param>
    /// <param name="targetPosition">目标位置</param>
    /// <param name="v">初始速度</param>
    /// <returns></returns>
    public static ProjectileMotion[] CalculateAngle(Vector3 initialPosition, Vector3 targetPosition, float v) => CalculateAngle(initialPosition, targetPosition, v, Physics.gravity);
    /// <summary>
    /// 给定初始速度大小，目标位置，求角度（复数解），不考虑重力xz方向分力和阻力
    /// </summary>
    /// <param name="initialPosition">初始位置</param>
    /// <param name="targetPosition">目标位置</param>
    /// <param name="v">初始速度</param>
    /// <param name="gravity">重力</param>
    /// <returns></returns>
    public static ProjectileMotion[] CalculateAngle(Vector3 initialPosition, Vector3 targetPosition, float v, Vector3 gravity)
    {
        var distance = targetPosition - initialPosition;

        var dirXZ = new Vector3(distance.x, 0f, distance.z).normalized;
        var x = distance.SetValue(null, 0, null).magnitude;
        var y = distance.y;

        var item = gravity.y * x * x / (2 * v * v);
        float a = item, b = x, c = item - y;

        var result = QuadraticEquation.Solve(a, b, c);
        if (result == null)
        {
            Debug.LogWarning("给定力度无法达到目标位置");
            return null;
        }
        var angle1 = Mathf.Atan(result[0]) * Mathf.Rad2Deg;
        var angle2 = Mathf.Atan(result[1]) * Mathf.Rad2Deg;

        //var root = b * b - (4f * a * c);
        //if (root < 0)
        //{
        //    Debug.LogWarning("给定力度无法达到目标位置");
        //    return null;
        //}

        //var angle1 = Mathf.Atan((-b + Mathf.Sqrt(root)) / (2f * a)) * Mathf.Rad2Deg;
        //var angle2 = Mathf.Atan((-b - Mathf.Sqrt(root)) / (2f * a)) * Mathf.Rad2Deg;
        var asix = Vector3.Cross(dirXZ, Vector3.up);

        var v0_1 = v * (Quaternion.AngleAxis(angle1, asix) * dirXZ);
        var v0_2 = v * (Quaternion.AngleAxis(angle2, asix) * dirXZ);

        return new ProjectileMotion[] { new ProjectileMotion(initialPosition, v0_1, Vector3.zero, Vector3.zero, gravity), new ProjectileMotion(initialPosition, v0_2, Vector3.zero, Vector3.zero, gravity) };
    }
    /// <summary>
    /// 给定抛射角度和目标位置，求抛射初始速度，不考虑重力xz方向分力和阻力
    /// </summary>
    /// <param name="initialPosition">初始位置</param>
    /// <param name="targetPosition">目标位置</param>
    /// <param name="angle">抛射角</param>
    /// <returns></returns>
    public static ProjectileMotion CalculateVelocity(Vector3 initialPosition, Vector3 targetPosition, float angle) => CalculateVelocity(initialPosition, targetPosition, angle, Physics.gravity);
    /// <summary>
    /// 给定抛射角度和目标位置，求抛射初始速度，不考虑重力xz方向分力和阻力
    /// </summary>
    /// <param name="initialPosition">初始位置</param>
    /// <param name="targetPosition">目标位置</param>
    /// <param name="angle">抛射角</param>
    /// <param name="gravity">重力</param>
    /// <returns></returns>
    public static ProjectileMotion CalculateVelocity(Vector3 initialPosition, Vector3 targetPosition, float angle, Vector3 gravity)
    {
        var distance = targetPosition - initialPosition;

        var dirXZ = new Vector3(distance.x, 0f, distance.z).normalized;
        var x = distance.SetValue(null, 0f, null).magnitude;
        var y = distance.y;

        var tanAngle = Mathf.Tan(angle * Mathf.Deg2Rad);
        var a = (tanAngle * x) - y;
        var b = 0f;
        var c = 0.5f * gravity.y * x * x * (1 + tanAngle * tanAngle);

        var root = b * b - (4f * a * c);
        if (root < 0)
        {
            Debug.LogWarning("给定角度无法达到目标位置");
            return new ProjectileMotion();
        }

        var v = Mathf.Max((-b + Mathf.Sqrt(root)) / (2f * a), (-b - Mathf.Sqrt(root)) / (2f * a));

        var asix = Vector3.Cross(dirXZ, Vector3.up);
        var v0 = v * (Quaternion.AngleAxis(angle, asix) * dirXZ);
        return new ProjectileMotion(initialPosition, v0, Vector3.zero, Vector3.zero, gravity);
    }

    public ProjectileMotion(Vector3 initialPosition, Vector3 initialVelocity) : this(initialPosition, initialVelocity, Vector3.zero, Vector3.zero, Physics.gravity) { }
    public ProjectileMotion(Vector3 initialPosition, Vector3 initialVelocity, Vector3 drag, Vector3 dragA, Vector3 gravity)
    {
        this.InitialPosition = initialPosition;
        this.InitialVelocity = initialVelocity;
        this.Drag = drag;
        this.DragA = dragA;
        this.Gravity = gravity;
    }
}

/// <summary>
/// 一元二次方程
/// </summary>
public struct QuadraticEquation
{
    public static float[] Solve(float a, float b, float c)
    {
        if (a == 0) return null;

        var delta = b * b - 4 * a * c;
        if (delta < 0) return null;
        var result = new float[2];

        delta = Mathf.Sqrt(delta);
        result[0] = (-b + delta) / (2 * a);
        result[1] = (-b - delta) / (2 * a);
        return result;
    }
    public static float SolveY(float a, float b, float c, float x) => a * x * x + b * x + c;
    public static float[] SolveX(float a, float b, float c, float y) => Solve(a, b, c - y);
}