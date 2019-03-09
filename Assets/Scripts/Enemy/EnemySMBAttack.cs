using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySMBAttack : StateMachineBehaviour
{
    private Enemy enemy = null;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        animator.ResetTrigger(Enemy.AnimAttackHash);
    }
    public override void OnStateExit(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        if (this.enemy == null)
        {
            this.enemy = animator.GetComponent<Enemy>();
            if (this.enemy == null) return;
        }
        if (this.enemy.StateMachine.CurrentState is EnemyAttackState)
        {
            var state = this.enemy.StateMachine.CurrentState as EnemyAttackState;
            state.IsAttackOver = true;
        }
    }

}

