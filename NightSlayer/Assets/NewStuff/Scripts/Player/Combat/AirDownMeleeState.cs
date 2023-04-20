using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirDownMeleeState : MeleeBaseState
{
    public override void OnEnter(StateMachine _stateMachine)
    {
        base.OnEnter(_stateMachine);

        //Attack
        attackIndex = 6;
        attackDamage = 10;
        knockbackForceX = 1;
        knockbackForceY = 1;
        camShakeIntensity = 0.2f;
        duration = 0.2f;
        animator.SetTrigger("AirDownAttack");
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