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
        knockbackForceX = 1;
        knockbackForceY = 10;
        camShakeIntensity = 0.2f;
        duration = 0.3f;
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