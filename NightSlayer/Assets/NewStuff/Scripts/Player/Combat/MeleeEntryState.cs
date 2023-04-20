using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeEntryState : State
{
    private PlayerScript playerScript;

    public override void OnEnter(StateMachine _stateMachine)
    {
        base.OnEnter(_stateMachine);
        playerScript = _stateMachine.GetComponent<PlayerScript>();

        State nextState = (playerScript && playerScript.Grounded) ? (State)new GroundEntryState() : (State)new AirUpMeleeState();
        stateMachine.SetNextState(nextState);
    }
}