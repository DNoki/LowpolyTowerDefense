using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 敵のジェネレーター
/// </summary>
[System.Serializable]
public class EnemyGenerator
{
    /// <summary>
    /// 敌人预制体
    /// </summary>
    [Tooltip("敌人预制体")] public Enemy Prefab = null;
    /// <summary>
    /// 生成间隔时间
    /// </summary>
    [Tooltip("生成间隔时间")] public Vector2 IntervalTimeRange = new Vector2(1f, 2f);
    /// <summary>
    /// 生成数量
    /// </summary>
    [Tooltip("生成数量")] public int GenerateQuantity = 10;
    /// <summary>
    /// 初始等待时间
    /// </summary>
    [Tooltip("初始等待时间")] public float InitialWaitTime = 0f;

    [Tooltip("使用预制体数据")] public bool UseUnityPrefab = true;

    /// <summary>
    /// 回合
    /// </summary>
    [Header("若使用预制体则以下设置无效")] public int Round = 1;
    public float HPLimit = 10f;
    public int DropMoney = 10;
    public int DropScore = 10;
    public float MoveSpeed = 3f;
    public float RotateSpeed = 3f;
    public float AttackPower = 10f;
    public float AttackDistance = 1f;
    public float AttackSpeed = 1f;
    public float Scale = 1f;
    public string Reserve1 = string.Empty;
    public string Reserve2 = string.Empty;
    public string Reserve3 = string.Empty;

    /// <summary>
    /// 等待计时器
    /// </summary>
    private Timer waitTimer = null;
    /// <summary>
    /// 等待是否结束
    /// </summary>
    private bool isWaitingOver = false;

    /// <summary>
    /// 生成间隔计时器
    /// </summary>
    public Timer IntervalTimer { get; private set; } = null;
    /// <summary>
    /// 已生成数量
    /// </summary>
    public int AlreadyQuantity { get; set; } = 0;
    /// <summary>
    /// 是否已经结束
    /// </summary>
    public bool IsOver => this.AlreadyQuantity >= this.GenerateQuantity;

    /// <summary>
    /// 是否正在等待
    /// </summary>
    /// <returns></returns>
    public bool UpdateAndReachIsWaiting()
    {
        if (this.isWaitingOver) return false;

        if (this.waitTimer.UpdateAndIsReach(Time.deltaTime))
        {
            this.isWaitingOver = true;
            return false;
        }
        else return true;
    }
    /// <summary>
    /// 重置
    /// </summary>
    public void Reset()
    {
        this.isWaitingOver = false;
        if (this.waitTimer == null) this.waitTimer = new Timer(this.InitialWaitTime);
        else this.waitTimer.DefiniteTime = this.InitialWaitTime;

        var intervalTime = Random.Range(this.IntervalTimeRange.x, this.IntervalTimeRange.y);
        if (this.IntervalTimer == null) this.IntervalTimer = new Timer(intervalTime);
        else this.IntervalTimer.DefiniteTime = intervalTime;
        this.IntervalTimer.Reset();

        this.AlreadyQuantity = 0;
    }

    public static EnemyGenerator CreateEnemyGenerator(GameScene gameScene, Dictionary<string, string> data)
    {
        var generator = new EnemyGenerator();

        generator.UseUnityPrefab = false;

        generator.Round = data["Round"].ToInt32();
        generator.Prefab = gameScene.EnemyFactory.TakeEnemyForType(data["Name"]);
        generator.IntervalTimeRange = new Vector2(data["MinIntervalTime"].ToFloat(), data["MaxIntervalTime"].ToFloat());
        generator.GenerateQuantity = data["Count"].ToInt32();
        generator.InitialWaitTime = data["InitialWaitTime"].ToFloat();

        generator.HPLimit = data["HPLimit"].ToFloat();
        generator.DropMoney = data["DropMoney"].ToInt32();
        generator.DropScore = data["DropScore"].ToInt32();
        generator.MoveSpeed = data["MoveSpeed"].ToFloat();
        generator.RotateSpeed = data["RotateSpeed"].ToFloat();
        generator.AttackPower = data["AttackPower"].ToFloat();
        generator.AttackDistance = data["AttackDistance"].ToFloat();
        generator.AttackSpeed = data["AttackSpeed"].ToFloat();
        generator.Scale = data["Scale"].ToFloat();
        generator.Reserve1 = data["Reserve1"];
        generator.Reserve2 = data["Reserve2"];
        generator.Reserve3 = data["Reserve3"];

        return generator;
    }
}

