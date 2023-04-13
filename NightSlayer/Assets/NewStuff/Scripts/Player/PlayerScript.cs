using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public class PlayerScript : MonoBehaviour
{

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

    [Header("Events")]
    [Space]

    public UnityEvent OnLandEvent;

    private void Awake()
    {
        if (OnLandEvent == null)
            OnLandEvent = new UnityEvent();

        m_initialGravity = m_body2d.gravityScale;

        m_running = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.D))
        {
            float timeSinceLastTap = Time.time - m_lastTapTime;

            if (timeSinceLastTap <= DOUBLE_TAP_TIME)
            {
                m_doubleTap = true;
            }
            else
            {
                m_doubleTap = false;
            }
            m_lastTapTime = Time.time;
        }

        if (Input.GetKey(KeyCode.D) && m_doubleTap)
        {
            m_running = true;
        }
        else if (Input.GetKeyUp(KeyCode.D) && m_doubleTap)
        {
            m_running = false;
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            float timeSinceLastTap = Time.time - m_lastTapTime;

            if (timeSinceLastTap <= DOUBLE_TAP_TIME)
            {
                m_doubleTap = true;
            }
            else
            {
                m_doubleTap = false;
            }
            m_lastTapTime = Time.time;
        }

        if (Input.GetKey(KeyCode.A) && m_doubleTap)
        {
            m_running = true;
        }
        else if (Input.GetKeyUp(KeyCode.A) && m_doubleTap)
        {
            m_running = false;
        }

        m_animator.SetBool("Running", m_running);

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

        // -- Handle input and movement --
        m_inputX = Input.GetAxis("Horizontal");

        // Swap direction of sprite depending on walk direction
        if (m_inputX > 0)
        {
            GetComponent<SpriteRenderer>().flipX = false;
            m_facingDirection = 1;
            m_hitCollider.transform.localPosition = new Vector3(2f, m_hitCollider.transform.localPosition.y, m_hitCollider.transform.localPosition.z);
        }

        else if (m_inputX < 0)
        {
            GetComponent<SpriteRenderer>().flipX = true;
            m_facingDirection = -1;
            m_hitCollider.transform.localPosition = new Vector3(-2f, m_hitCollider.transform.localPosition.y, m_hitCollider.transform.localPosition.z);
        }

        //Set AirSpeed in animator
        m_animator.SetFloat("AirSpeedY", m_body2d.velocity.y);

        //Death
        if (Input.GetKeyDown("e") && !m_rolling)
        {
            m_animator.SetTrigger("Death");
        }

        //Hurt
        else if (Input.GetKeyDown("q") && !m_rolling)
            m_animator.SetTrigger("Hurt");

        // Block
        else if (Input.GetMouseButtonDown(1) && !m_rolling)
        {
            m_animator.SetTrigger("Block");
            m_animator.SetBool("IdleBlock", true);
        }

        else if (Input.GetMouseButtonUp(1))
            m_animator.SetBool("IdleBlock", false);

        //Run
        else if (Mathf.Abs(m_inputX) > Mathf.Epsilon)
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

        if (Input.GetKeyDown("left shift") && !m_rolling)
        {
            Roll();
        }

        if (Input.GetKeyDown("space") && (m_coyoteTimeCounter > 0) && !m_rolling)
        {
            Jump();
            m_coyoteTimeCounter = 0f;
        }

        if (m_running)
        {
            if (Input.GetMouseButtonDown(0) && !m_rolling && m_canDash && m_grounded)
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
        m_canMove = false;
        m_canDash = false;

        m_body2d.gravityScale = 0f;
        m_body2d.velocity = new Vector2(m_dashVelocity * m_facingDirection, 0f);

        yield return new WaitForSeconds(m_dashTime);
        m_canMove = true;

        m_body2d.gravityScale = m_initialGravity;

        yield return new WaitForSeconds(1f);

        m_canDash = true;
    }
}