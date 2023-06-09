using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirUpMeleeState : MeleeBaseState
{
    public override void OnEnter(StateMachine _stateMachine)
    {
        base.OnEnter(_stateMachine);

        //Attack
        attackIndex = 5;
        attackDamage = 10;
        knockbackForceX = 5;
        knockbackForceY = 30;
        camShakeIntensity = 0.2f;
        duration = 0.38f;
        animator.SetTrigger("AirUpAttack");
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