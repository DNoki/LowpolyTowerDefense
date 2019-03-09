using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySMBHit : StateMachineBehaviour
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        animator.ResetTrigger(Enemy.AnimHitHash);
    }
}
