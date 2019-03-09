using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSceneFree : GameScene
{
    /// <summary>
    /// 怪物生成距离领域范围
    /// </summary>
    [SerializeField] private float generateDistance = 30f;
    private Circle generateCircle = new Circle();

    /// <summary>
    /// 100以内质数表
    /// </summary>
    private readonly int[] primeNumbers = new int[] { 2, 3, 5, 7, 11, 13, 17, 19, 23, 29, 31, 37, 41, 43, 47, 53, 59, 61, 67, 71, 73, 79, 83, 89, 97 };

    public override int TotalRound => this.primeNumbers.Length;

    protected override IEnumerator StartNextRound()
    {
        this.isGenerateOver = false;
        this.CurrentRound++;
        this.onRoundChange.Invoke(this.CurrentRound, this.TotalRound);
        yield return new WaitForSeconds(this.RoundIntervalTimeList[this.CurrentRound - 1]);

        //var generateQuantity = primeNumbers[this.CurrentRound - 1];// 生成数量

        //var i = 0;
        //var enemyPrefab = this.EnemyPrefabOfRound[this.CurrentRound - 1].GetComponent<IEnemy>();
        //while (i++ < generateQuantity)
        //{
        //    Vector3 pos = Vector3.zero;
        //    while (pos == Vector3.zero)
        //        pos = Random.insideUnitCircle;
        //    pos = pos.normalized;
        //    pos = this.generateCircle.ClosestPoint(new Vector3(pos.x, 0, pos.y));

        //    var enemy = this.EnemyFactory.TakeEnemyForType(enemyPrefab);
        //    enemy.Transform.position = new Vector3(pos.x, enemy.Transform.position.y, pos.z);
        //    enemy.Transform.LookAt(this.CenterBuilding.transform);
        //    enemy.UpdatePathAsyn(this.CenterBuilding);

        //    // 生成一个敌人指示方向
        //    var enemyDirectionEffect =GameObjectFactory.EnemyDirectionEffectPoolObject.Take();
        //    enemyDirectionEffect.Target = enemy;

        //    this.EnemyList.Add(enemy);
        //    yield return new WaitForSeconds(this.GenerateIntervalTime[this.CurrentRound - 1].RandomNumber());
        //}
        this.isGenerateOver = true;
    }

    ///// <summary>
    ///// 获取生成怪物数量
    ///// </summary>
    ///// <param name="round">回合</param>
    ///// <returns></returns>
    //public int GetGenerateQuantity(int round)
    //{
    //    round = Mathf.Clamp(round, 1, this.TotalRound);
    //    return this.primeNumbers[round - 1];
    //}
    /// <summary>
    /// 更新怪物生成圆半径
    /// </summary>
    public void UpdataGenerateCircle()
    {
        var rect = this.Ground.FieldArea.FieldRect;
        var radius = Mathf.Max(rect.min.x, rect.min.y, rect.max.x, rect.max.y);
        this.generateCircle.Radius = radius + this.generateDistance;
    }

}
