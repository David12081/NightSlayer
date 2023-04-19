using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PlayerScript : MonoBehaviour
{
    [SerializeField] private StateMachine meleeStateMachine;
    [SerializeField] private PlayerInput playerInput;
    private InputAction attackAction;
    [SerializeField] float m_speed = 4.0f;
    [SerializeField] float m_jumpForce = 7.5f;
    [SerializeField] float m_rollForce = 6.0f;

    [SerializeField] float m_coyoteTime;
    float m_coyoteTimeCounter;

    [SerializeField] Animator m_animator;
    [SerializeField] Rigidbody2D m_body2d;
    [SerializeField] GameObject m_hitCollider;

    [SerializeField] private LayerMask m_WhatIsGround;
    [SerializeField] private Transform m_GroundCheck;
    const float k_GroundedRadius = 0.2f;
    private bool m_grounded = false;
    public bool Grounded
    {
        get => m_grounded;
        set => m_grounded = value;
    }

    private bool m_rolling = false;
    private int m_facingDirection = 1;
    private float m_delayToIdle = 0.0f;
    private float m_rollDuration = 0.5f;
    private float m_rollCurrentTime;
    private float m_inputX;
    private float m_inputY;
    public float InputY
    {
        get => m_inputY;
        set => m_inputY = value;
    }

    private float m_lastTapTime;
    private bool m_doubleTap;
    private const float DOUBLE_TAP_TIME = 0.2f;
    private bool m_running;
    public bool Running
    {
        get => m_running;
        set => m_running = value;
    }

    [SerializeField] private float m_dashVelocity;
    [SerializeField] private float m_dashTime;
    private float m_initialGravity;

    private bool m_canDash = true;
    public bool CanDash
    {
        get => m_canDash;
        set => m_canDash = value;
    }

    private bool m_canMove = true;
    public bool CanMove
    {
        get => m_canMove;
        set => m_canMove = value;
    }

    private bool m_shielding = false;

    [Header("Events")]
    [Space]

    public UnityEvent OnLandEvent;

    private void Awake()
    {
        if (OnLandEvent == null)
            OnLandEvent = new UnityEvent();

        m_initialGravity = m_body2d.gravityScale;

        m_running = false;

        attackAction = playerInput.actions["Attack"];
    }

    public void OnMoveAxis(InputAction.CallbackContext value)
    {
        Vector2 input = value.ReadValue<Vector2>();
        m_inputX = input.x;
        m_inputY = input.y;
    }

    public void OnJump(InputAction.CallbackContext value)
    {
        if(value.started && (m_coyoteTimeCounter > 0) && !m_rolling)
        {
            Jump();
            m_coyoteTimeCounter = 0f;
        }
    }

    public void OnSprint(InputAction.CallbackContext value)
    {
        if(m_inputX != 0f)
        {
            if(value.started)
                m_running = true;
        }
    }

    public void OnShield(InputAction.CallbackContext value)
    {
        if(value.started)
        {
            m_shielding = true;
        }
        else if(value.performed)
        {
            m_shielding = false;
        }
    }

    void Update()
    {
        m_animator.SetBool("Running", m_running);

        if(m_inputX == 0f)
        {
            m_running = false;
        }
        
        if (m_running)
        {
            m_speed = 12.0f;
        }
        else
        {
            m_speed = 8.0f;
        }

        // Increase timer that checks roll duration
        if (m_rolling)
            m_rollCurrentTime += Time.deltaTime;

        // Disable rolling if timer extends duration
        if (m_rollCurrentTime > m_rollDuration)
        {
            m_rollCurrentTime = 0f;
            m_rolling = false;
        }

        m_animator.SetBool("Grounded", m_grounded);

        // Swap direction of sprite depending on walk direction
        if (m_inputX > 0f)
        {
            GetComponent<SpriteRenderer>().flipX = false;
            m_facingDirection = 1;
            m_hitCollider.transform.localPosition = new Vector3(2f, m_hitCollider.transform.localPosition.y, m_hitCollider.transform.localPosition.z);
        }

        else if (m_inputX < 0f)
        {
            GetComponent<SpriteRenderer>().flipX = true;
            m_facingDirection = -1;
            m_hitCollider.transform.localPosition = new Vector3(-2f, m_hitCollider.transform.localPosition.y, m_hitCollider.transform.localPosition.z);
        }

        //Set AirSpeed in animator
        m_animator.SetFloat("AirSpeedY", m_body2d.velocity.y);

        //Death
        //if (Input.GetKeyDown("e") && !m_rolling)
        //{
        //    m_animator.SetTrigger("Death");
        //}

        //Hurt
        //else if (Input.GetKeyDown("q") && !m_rolling)
        //    m_animator.SetTrigger("Hurt");

        // Block
        if (m_shielding && !m_rolling)
        {
            m_animator.SetBool("IdleBlock", true);
            m_canMove = false;
            if(m_inputX != 0f)
            {
                Roll();
            }
        }

        else if (!m_shielding)
        {
            m_canMove = true;
            m_animator.SetBool("IdleBlock", false);
        }

        //Run
        if (Mathf.Abs(m_inputX) > Mathf.Epsilon)
        {
            // Reset timer
            m_delayToIdle = 0.05f;
            m_animator.SetInteger("AnimState", 1);
        }

        //Idle
        else
        {
            // Prevents flickering transitions to idle
            m_delayToIdle -= Time.deltaTime;
            if (m_delayToIdle < 0)
                m_animator.SetInteger("AnimState", 0);
        }

        if (!m_rolling && m_canMove)
            Move();

        if (m_running)
        {
            if (attackAction.ReadValue<float>() == 1 && !m_rolling && m_canDash && m_grounded
                && meleeStateMachine.CurrentState.GetType() == typeof(IdleCombatState))
            {
                StartCoroutine(Dash());
            }
        }

        if (m_grounded)
        {
            m_coyoteTimeCounter = m_coyoteTime;
        }
        else
        {
            m_coyoteTimeCounter -= Time.deltaTime;
        }
    }

    private void FixedUpdate()
    {
        bool wasGrounded = m_grounded;
        m_grounded = false;

        Collider2D[] colliders = Physics2D.OverlapCircleAll(m_GroundCheck.position, k_GroundedRadius, m_WhatIsGround);
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].gameObject != gameObject)
            {
                m_grounded = true;

                if (!wasGrounded)
                {
                    OnLandEvent.Invoke();
                }
            }
        }
    }

    void Move()
    {
        m_body2d.velocity = new Vector2(m_inputX * m_speed, m_body2d.velocity.y);
    }

    void Roll()
    {
        m_shielding = false;
        m_rolling = true;
        m_animator.SetTrigger("Roll");
        m_body2d.velocity = transform.right * m_facingDirection * m_rollForce;
    }

    void Jump()
    {
        m_animator.SetTrigger("Jump");
        m_grounded = false;
        m_animator.SetBool("Grounded", m_grounded);
        m_body2d.velocity = new Vector2(m_body2d.velocity.x, m_jumpForce);
    }

    IEnumerator Dash()
    {
        m_body2d.velocity = new Vector2(m_dashVelocity * m_facingDirection, 0f);
        meleeStateMachine.SetNextState(new GroundDashAttackState());
        m_canMove = false;
        m_body2d.gravityScale = 0f;
        m_canDash = false;

        yield return new WaitForSeconds(m_dashTime);
        m_canMove = true;

        m_body2d.gravityScale = m_initialGravity;

        yield return new WaitForSeconds(1f);

        m_canDash = true;
        m_running = false;
    }
}