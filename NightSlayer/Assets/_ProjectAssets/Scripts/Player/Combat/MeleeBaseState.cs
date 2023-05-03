using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MeleeBaseState : State
{
    // How long this state should be active for before moving on
    public float duration;
    // Cached animator component
    protected Animator animator;
    // bool to check whether or not the next attack in the sequence should be played or not
    protected bool shouldCombo = false;
    // The attack index in the sequence of attacks
    protected int attackIndex;

    // The attack damage in the sequence of attacks
    protected int attackDamage;
    // The attack X-axis knockback force in the sequence of attacks
    protected int knockbackForceX;
    // The attack Y-axis knockback force in the sequence of attacks
    protected int knockbackForceY;

    // The attack Y-axis knockback force in the sequence of attacks
    protected float camShakeIntensity;

    // The cached hit collider component of this attack
    protected Collider2D hitCollider;
    // Cached already struck objects of said attack to avoid overlapping attacks on same target
    private List<Collider2D> collidersDamaged;
    // The Hit Effect to Spawn on the afflicted Enemy
    private GameObject HitEffectPrefab;

    // Input buffer Timer
    private float AttackPressedTimer = 0f;

    private PlayerScript playerScript;

    private PlayerInput playerInput;
    private InputAction attackAction;

    private AttackDetails attackDetails;

    public override void OnEnter(StateMachine _stateMachine)
    {
        base.OnEnter(_stateMachine);
        animator = GetComponent<Animator>();
        collidersDamaged = new List<Collider2D>();
        hitCollider = GetComponent<ComboCharacter>().hitbox;
        HitEffectPrefab = GetComponent<ComboCharacter>().Hiteffect;
        playerInput = GetComponent<PlayerInput>();
        attackAction = playerInput.actions["Attack"];
        playerScript = animator.GetComponent<PlayerScript>();
        AttackPressedTimer = 0f;
        attackAction.started += AttackTImer;
        attackAction.performed += AttackTImer;
        attackDetails.position = animator.transform.position;
        attackDetails.stunDamageAmount = 1f;
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
        AttackPressedTimer -= Time.deltaTime;

        if (animator.GetFloat("Weapon.Active") > 0f)
        {
            Attack();
        }

        if (animator.GetFloat("AttackWindow.Open") > 0f && AttackPressedTimer > 0f)
        {
            shouldCombo = true;
        }
        else if(animator.GetFloat("AttackWindow.Open") < 0f && AttackPressedTimer < 0f)
        {
            shouldCombo = false;
        }
    }

    void AttackTImer(InputAction.CallbackContext context)
    {
        if(context.started)
        {
            AttackPressedTimer = 2f;
        }
        else if(context.performed)
        {
            attackAction.started -= AttackTImer;
            attackAction.performed -= AttackTImer;
        }
    }

    public override void OnExit()
    {
        base.OnExit();
    }

    protected void Attack()
    {
        Collider2D[] collidersToDamage = new Collider2D[10];
        ContactFilter2D filter = new ContactFilter2D();
        filter.useTriggers = true;
        int colliderCount = Physics2D.OverlapCollider(hitCollider, filter, collidersToDamage);

        attackDetails.damageAmount = attackDamage;
        attackDetails.knockbackForceX = knockbackForceX;
        attackDetails.knockbackForceY = knockbackForceY;
        
        for (int i = 0; i < colliderCount; i++)
        {
            if (!collidersDamaged.Contains(collidersToDamage[i]))
            {
                TeamComponent hitTeamComponent = collidersToDamage[i].GetComponent<TeamComponent>();

                // Only check colliders with a valid Team Componnent attached
                if (hitTeamComponent && hitTeamComponent.teamIndex == TeamIndex.Enemy)
                {
                    hitTeamComponent.transform.SendMessage("Damage", attackDetails);

                    CinemachineShake.Instance.ShakeCamera(camShakeIntensity, 0.1f);

                    HitStopController.Instance.Stop(0.1f);

                    GameObject.Instantiate(HitEffectPrefab, collidersToDamage[i].transform);
                    Debug.Log("Enemy Has Taken: " + attackDamage + " Damage");
                    collidersDamaged.Add(collidersToDamage[i]);

                    if (stateMachine.CurrentState.GetType() == typeof(AirDownMeleeState))
                        playerScript.HopAttack();
                }
            }
        }
    }
}