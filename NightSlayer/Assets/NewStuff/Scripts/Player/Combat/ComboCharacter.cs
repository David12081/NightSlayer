using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComboCharacter : MonoBehaviour
{
    [SerializeField] private StateMachine meleeStateMachine;
    [SerializeField] private PlayerScript playerScript;

    public Collider2D hitbox;
    public GameObject Hiteffect;

    void Update()
    {
        if (Input.GetMouseButton(0) && !playerScript.Running && meleeStateMachine.CurrentState.GetType() == typeof(IdleCombatState))
        {
            meleeStateMachine.SetNextState(new GroundEntryState());
        }

        else if (Input.GetMouseButton(0) && playerScript.CanDash && playerScript.Grounded && meleeStateMachine.CurrentState.GetType() == typeof(IdleCombatState))
        {
            meleeStateMachine.SetNextState(new GroundDashAttackState());
        }
    }
}