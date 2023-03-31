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
        duration = 0.5f;
        animator.SetTrigger("DashAttack");
        Debug.Log("Player Dash Attack" + " Fired!");
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