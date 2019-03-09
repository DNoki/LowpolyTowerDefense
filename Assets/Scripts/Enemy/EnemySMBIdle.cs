using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySMBIdle : StateMachineBehaviour
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        animator.ResetTrigger(Enemy.AnimResetHash);
    }
}
