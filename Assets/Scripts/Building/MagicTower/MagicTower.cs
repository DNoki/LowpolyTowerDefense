using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 魔法塔
/// </summary>
public class MagicTower : Tower, IPoolObject, IMessageBox
{
    string IMessageBox.GetMessageText
    {
        get
        {
            var aS = this.AttackSystem as MagicTowerAttackSystem;
            if (this.CurrentLevel < BuildingFactory.BUILDING_LEVEL_LIMIT)
                return $"魔法塔  Lv：{this.CurrentLevel}\r\n\r\n攻撃力：{aS.AttackPower}\r\n同時に攻撃できる数：{aS.MaxAttackQuantity}\r\nアップグレードコスト：{this.upgradeRequired[this.CurrentLevel - 1]}";
            else return $"魔法塔  Lv：{this.CurrentLevel}\r\n\r\n攻撃力：{aS.AttackPower}\r\n同時に攻撃できる数：{aS.MaxAttackQuantity}";
        }
    }

    void IPoolObject.Initialize()
    {
        CommonInitialize();
    }
    void IPoolObject.Release()
    {
        CommonRelease();
        (this.AttackSystem as MagicTowerAttackSystem).ClearLine();
    }

    public override void Upgrade()
    {
        var level = this.CurrentLevel;
        base.Upgrade();
        if (level != this.CurrentLevel)
            (this.AttackSystem as MagicTowerAttackSystem).ClearLine();
    }
}
