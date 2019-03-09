using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSceneNormal : GameScene
{
    [SerializeField] private Route route = null;

    private int totalRound = 25;

    /// <summary>
    /// 敌人移动路径
    /// </summary>
    public Route Route => this.route == null ? (this.route = FindObjectOfType<Route>()) : this.route;
    public override int TotalRound => this.totalRound;

    protected override IEnumerator StartNextRound()
    {
        this.isGenerateOver = false;
        this.CurrentRound++;
        this.onRoundChange.Invoke(this.CurrentRound, this.TotalRound);
        yield return new WaitForSeconds(this.RoundIntervalTimeList[this.CurrentRound - 1]);

        var serialNumber = 0;// 序列号

        if (this.CSVGeneratorsList != null)
        {
            // 从CSV文件生成怪物
            Debug.Log("CSVファイルから生成");
            var generatorsList = this.CSVGeneratorsList.FindAll(g => g.Round == this.CurrentRound);
            for (var i = 0; i < generatorsList.Count; i++)            
                generatorsList[i].Reset();

            while (true)
            {
                var isOver = true;

                for (var i = 0; i < generatorsList.Count; i++)
                {
                    var generator = generatorsList[i];
                    if (generator.IsOver)
                        continue;
                    if (generator.UpdateAndReachIsWaiting())
                    {
                        isOver = false;
                        continue;
                    }
                    if (generator.IntervalTimer.UpdateAndIsReach(Time.deltaTime))
                    {
                        generator.AlreadyQuantity++;

                        var enemy = this.EnemyFactory.TakeEnemyForType(generator.Prefab);
                        enemy.AfterTakedInitialize(generator);
                        enemy.SerialNumber = serialNumber++;
                        enemy.Transform.position = this.Route.RouteList[0].position.SetValue(y: enemy.OffsetY);
                        enemy.Transform.LookAt(this.Route.RouteList[1]);

                        AddEnemy(enemy);
                    }
                    isOver = false;
                }
                if (isOver) break;
                yield return null;
            }
        }
        else
        {
            var generatorsList = this.CSVGeneratorsList.FindAll(g => g.Round == this.CurrentRound);
            foreach (var g in generatorsList)
            {
                g.Reset();
            }

            while (true)
            {
                var isOver = true;
                for (var i = 0; i < generatorsList.Count; i++)
                {
                    var generator = generatorsList[i];
                    if (generator.IsOver || generator.UpdateAndReachIsWaiting())
                        continue;
                    if (generator.IntervalTimer.UpdateAndIsReach(Time.deltaTime))
                    {
                        generator.AlreadyQuantity++;

                        var enemy = this.EnemyFactory.TakeEnemyForType(generator.Prefab);
                        if (generator.UseUnityPrefab)
                            enemy.AfterTakedInitialize(generator.Prefab);
                        else enemy.AfterTakedInitialize(generator);
                        enemy.SerialNumber = serialNumber++;
                        enemy.Transform.position = this.Route.RouteList[0].position.SetValue(y: enemy.OffsetY);
                        enemy.Transform.LookAt(this.Route.RouteList[1]);

                        AddEnemy(enemy);
                    }
                    isOver = false;
                }
                if (isOver) break;
                yield return null;
            }
        }
        this.isGenerateOver = true;
    }

    protected override void Awake()
    {
        base.Awake();
        if (this.CSVGeneratorsList != null)
        {
            var tr = 0;
            foreach (var generator in this.CSVGeneratorsList)
            {
                tr = Mathf.Max(generator.Round, tr);
            }
            this.totalRound = tr;
        }
    }
}
