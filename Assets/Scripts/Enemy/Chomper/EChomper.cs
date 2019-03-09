using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class EChomper : Enemy, IPoolObject
{ 
    void IPoolObject.Initialize()
    {
        CommonInitialize();
        if (this.BehaviourMode == GameMode.FREE)
        {
            this.StateMachine = new FiniteStateMachine<Enemy>(this, new EnemyIdleState());
        }
    }
    void IPoolObject.Release()
    {
        CommonRelease();
    }

    /// <summary>
    /// 死亡，由动画调用
    /// </summary>
    protected override void ReleaseThis()
    {
        EnemyFactory.EChomperPoolObject.Release(this);
    }

    public override void ApplyDamage(DamageMessage data)
    {
        if (this.IsDie) return;
        base.ApplyDamage(data);
        if (this.IsDie)
        {
            AudioManager.Instance.PlayInstance("DogDie");
            this.Anim.SetTrigger(AnimDieHash);
        }
        else
        {
            if (this.BehaviourMode == GameMode.FREE)
                this.Anim.SetTrigger(AnimHitHash);
        }
    }
}