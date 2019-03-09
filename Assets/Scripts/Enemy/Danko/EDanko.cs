using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EDanko : Enemy, IPoolObject
{
    void IPoolObject.Initialize()
    {
        CommonInitialize();
    }
    void IPoolObject.Release()
    {
        CommonRelease();
    }
    public override IEnemy AfterTakedInitialize(IEnemy enemy)
    {
        base.AfterTakedInitialize(enemy);
        //var e = enemy as EDodo;

        return this;
    }
    public override IEnemy AfterTakedInitialize(EnemyGenerator generator)
    {
        base.AfterTakedInitialize(generator);

        return this;
    }

    protected override void ReleaseThis()
    {
        EnemyFactory.EDankoPoolObject.Release(this);
    }

    public override void ApplyDamage(DamageMessage data)
    {
        if (this.IsDie) return;
        base.ApplyDamage(data);
        if (this.IsDie)
        {
            this.Anim.SetTrigger(AnimDieHash);
        }
    }
}
