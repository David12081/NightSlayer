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
        attackDamage = 5;
        knockbackForceX = 5;
        knockbackForceY = 35;
        camShakeIntensity = 3f;
        duration = 0.45f;
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