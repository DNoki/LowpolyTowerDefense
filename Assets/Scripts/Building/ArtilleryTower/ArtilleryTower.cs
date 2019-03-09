using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 炮塔
/// 砲塔
/// </summary>
public class ArtilleryTower : Tower, IPoolObject, IMessageBox
{
    string IMessageBox.GetMessageText
    {
        get
        {
            var aS = this.AttackSystem as ArtilleryTowerAttackSystem;
            if (this.CurrentLevel < BuildingFactory.BUILDING_LEVEL_LIMIT)
                return $"砲塔  Lv：{this.CurrentLevel}\r\n\r\n攻撃力：{aS.AttackPower}\r\n攻撃スピード：{aS.AttackSpeed}\r\n爆発範囲：{aS.ExplosionRange}\r\nアップグレードコスト：{this.upgradeRequired[this.CurrentLevel - 1]}";
            else return $"砲塔  Lv：{this.CurrentLevel}\r\n\r\n攻撃力：{aS.AttackPower}\r\n攻撃スピード：{aS.AttackSpeed}\r\n爆発範囲：{aS.ExplosionRange}";
        }
    }

    void IPoolObject.Initialize()
    {
        CommonInitialize();
    }
    void IPoolObject.Release()
    {
        CommonRelease();
    }
}
