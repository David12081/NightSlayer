using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ComboCharacter : MonoBehaviour
{
    [SerializeField] private StateMachine meleeStateMachine;
    [SerializeField] private PlayerScript playerScript;
    [SerializeField] private PlayerInput input;
    private InputAction attackAction;

    public Collider2D hitbox;
    public GameObject Hiteffect;

    private void Awake()
    {
        attackAction = input.actions["Attack"];
    }

    void Update()
    {
        if (attackAction.ReadValue<float>() == 1 && !playerScript.Running && meleeStateMachine.CurrentState.GetType() == typeof(IdleCombatState))
        {
            meleeStateMachine.SetNextState(new GroundEntryState());
        }
    }
}