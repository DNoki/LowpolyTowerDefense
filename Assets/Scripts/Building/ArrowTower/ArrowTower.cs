using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 箭塔
/// 矢塔
/// </summary>
public class ArrowTower : Tower, IPoolObject, IMessageBox
{
    string IMessageBox.GetMessageText
    {
        get
        {
            var aS = this.AttackSystem as ArrowTowerAttackSystem;
            if (this.CurrentLevel < BuildingFactory.BUILDING_LEVEL_LIMIT)
                return $"矢塔  Lv：{this.CurrentLevel}\r\n\r\n攻撃力：{aS.AttackPower}\r\n攻撃スピード：{aS.AttackSpeed}\r\nアップグレードコスト：{this.upgradeRequired[this.CurrentLevel - 1]}";
            else return $"矢塔  Lv：{this.CurrentLevel}\r\n\r\n攻撃力：{aS.AttackPower}\r\n攻撃スピード：{aS.AttackSpeed}";
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
