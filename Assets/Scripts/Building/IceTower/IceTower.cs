using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 氷塔
/// </summary>
public class IceTower : Tower, IPoolObject, IMessageBox
{
    string IMessageBox.GetMessageText
    {
        get
        {
            var aS = this.AttackSystem as IceTowerAttackSystem;
            if (this.CurrentLevel < BuildingFactory.BUILDING_LEVEL_LIMIT)
                return $"氷塔  Lv：{this.CurrentLevel}\r\n\r\n攻撃スピード：{aS.AttackSpeed}\r\n減速強度：{aS.DecelerationPower}\r\n減速時間：{aS.DecelerationTime}\r\nアップグレードコスト：{this.upgradeRequired[this.CurrentLevel - 1]}";
            else return $"氷塔  Lv：{this.CurrentLevel}\r\n\r\n攻撃スピード：{aS.AttackSpeed}\r\n減速強度：{aS.DecelerationPower}\r\n減速時間：{aS.DecelerationTime}";
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