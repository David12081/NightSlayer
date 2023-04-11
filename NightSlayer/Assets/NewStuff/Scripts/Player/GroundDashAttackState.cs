using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundDashAttackState : MeleeBaseState
{
    public override void OnEnter(StateMachine _stateMachine)
    {
        base.OnEnter(_stateMachine);

        //Attack
        attackIndex = 4;
        attackDamage = 10;
        knockbackForceX = 5;
        knockbackForceY = 35;
        duration = 0.5f;
        animator.SetTrigger("DashAttack");
    }

    public override void OnUpdate()
    {
        base.OnUpdate();

        if (fixedtime >= duration)
        {
            stateMachine.SetNextStateToMain();
        }
    }
}