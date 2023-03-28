using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour, ISaveable
{
	public static PlayerController instance;
	[SerializeField] private float m_JumpForce;                                 // Amount of force added when the player jumps.
	[SerializeField] private float m_doubleJumpForce;
	[Range(0, .3f)][SerializeField] private float m_MovementSmoothing = .05f;   // How much to smooth out the movement
	[SerializeField] private LayerMask m_WhatIsGround;                          // A mask determining what is ground to the character
	[SerializeField] private Transform m_GroundCheck;                           // A position marking where to check if the player is grounded.

	const float k_GroundedRadius = 0.2f;                                        // Radius of the overlap circle to determine if grounded
	private bool m_Grounded;                                                    // Whether or not the player is grounded.
	private Rigidbody2D m_Rigidbody2D;
	private Vector3 m_Velocity = Vector3.zero;

	private float normMovementX, normMovementY;
	[SerializeField] float movementSpeed;
	bool canJump;
	public bool canDoubleJump;
	bool jumping;

	public Transform attackAnchor, attackPos;
	[SerializeField] float startTimeBetweenAttacks;
	float timeBetweenAttacks;
	public LayerMask whatIsEnemies;
	public float attackRange;
	public float attackDamage;
	bool canAttack;
	bool airAttack;

	private PlayerHealth playerHealth;

	bool canFuryMode = true;
	public GameObject particle;

	public bool activeDash;
	bool isDashing, canDash;
	float dashingDir;
	[SerializeField] float dashingVelocity;
	private float dashingTimer;
	[SerializeField] float startDashTimer;

	public bool canWallJump;
	bool isTouchingFront;
	[SerializeField] Transform frontWallCheck;
	bool wallSliding;
	[SerializeField] float wallSlidingSpeed;

	bool wallJumping;
	[SerializeField] float xWallForce;
	[SerializeField] float yWallForce;
	[SerializeField] float wallJumpTimer;

	[SerializeField] float coyoteTime;
	float coyoteTimeCounter;

	[SerializeField] float knockbackVel, knockbackTimer;
	bool knockbacked;

	[HideInInspector]
	public bool interact;

	private SaveLoadSystem saveLoadSystem;
	[SerializeField] ParticleSystem dustParticles;

	//Animation States
	[SerializeField] Animator anim;
	private string currentState;
	const string PLAYER_IDLE = "Idle";
	const string PLAYER_RUN = "Run";
	const string PLAYER_JUMP = "Jump";
	const string PLAYER_FALLING = "Falling";

	[Header("Events")]
	[Space]

	public UnityEvent OnLandEvent;

	void OnEnable()
    {
		saveLoadSystem.Load();
	}

	private void Awake()
	{
		m_Rigidbody2D = GetComponent<Rigidbody2D>();

		playerHealth = GetComponent<PlayerHealth>();

		saveLoadSystem = GetComponent<SaveLoadSystem>();

		if (instance == null)
		{
			instance = this;
		}
		else
		{
			Destroy(gameObject);
		}
		DontDestroyOnLoad(gameObject);

		particle.SetActive(false);

		if (OnLandEvent == null)
			OnLandEvent = new UnityEvent();
	}

    public void OnMoveLeft(InputAction.CallbackContext value)
    {
		float inputMovement = value.ReadValue<float>();
		normMovementX = inputMovement * -1;

		if (normMovementX != 0f)
		{
			dashingDir = normMovementX;
		}
	}

	public void OnMoveRight(InputAction.CallbackContext value)
	{
		float inputMovement = value.ReadValue<float>();
		normMovementX = inputMovement;

		if (normMovementX != 0f)
		{
			dashingDir = normMovementX;
		}
	}

	public void OnMoveUp(InputAction.CallbackContext value)
	{
		float inputMovement = value.ReadValue<float>();
		normMovementY = inputMovement;
	}

	public void OnMoveDown(InputAction.CallbackContext value)
	{
		float inputMovement = value.ReadValue<float>();
		normMovementY = inputMovement;
		normMovementY *= -1;
	}


	public void OnAttack(InputAction.CallbackContext value)
    {
		if(value.started && canAttack)
        {
			Attack();
			timeBetweenAttacks = startTimeBetweenAttacks;

			if(normMovementY > 0f && !m_Grounded && airAttack)
            {
				airAttack = false;
				m_Rigidbody2D.velocity = Vector2.up * m_JumpForce;
			}
		}
	}

	public void Jump(InputAction.CallbackContext value)
    {
		if (value.performed && coyoteTimeCounter > 0)
		{
			jumping = true;
			m_Rigidbody2D.velocity = Vector2.up * m_JumpForce;
			canJump = false;
			coyoteTimeCounter = 0f;
			CreateDust();
		}
		else if(value.canceled && coyoteTimeCounter > 0)
        {
			jumping = true;
			m_Rigidbody2D.velocity = Vector2.up * (m_JumpForce + (m_JumpForce * 0.3f));
			canJump = false;
			coyoteTimeCounter = 0f;
			CreateDust();
		}
		if(canDoubleJump)
        {
			if (value.canceled && canJump)
			{
				canJump = false;
				m_Rigidbody2D.velocity = Vector2.up * m_doubleJumpForce;
				coyoteTimeCounter = 0f;
			}
		}

		if((value.performed || value.canceled) && wallSliding)
        {
			wallJumping = true;
			jumping = true;
			CreateDust();
			Invoke("SetWallJumpingToFalse", wallJumpTimer);
		}
	}

	public void FuryMode(InputAction.CallbackContext value)
    {
		if(canFuryMode)
        {
			if (value.started)
			{
				m_Rigidbody2D.constraints = RigidbodyConstraints2D.FreezePosition;
				//m_Rigidbody2D.constraints = RigidbodyConstraints2D.FreezeRotation;
			}
			else if (value.performed)
			{
				StartCoroutine(FuryModeRoutine());
				m_Rigidbody2D.constraints = RigidbodyConstraints2D.None;
				m_Rigidbody2D.constraints = RigidbodyConstraints2D.FreezeRotation;
			}
			else if (value.canceled)
			{
				m_Rigidbody2D.constraints = RigidbodyConstraints2D.None;
				m_Rigidbody2D.constraints = RigidbodyConstraints2D.FreezeRotation;
			}
		}
    }

	public void OnDash(InputAction.CallbackContext value)
    {
		if(activeDash)
        {
			if (canDash)
			{
				if (value.performed)
				{
					Dash();
					CreateDust();
				}
			}
		}
    }

    
	private void Update()
    {
		if (timeBetweenAttacks <= 0f)
        {
			canAttack = true;
        }
		else
        {
			canAttack = false;
			timeBetweenAttacks -= Time.deltaTime;
        }

		if(m_Grounded || isTouchingFront)
        {
			coyoteTimeCounter = coyoteTime;
			canJump = true;
			airAttack = true;
		}
        else
        {
			coyoteTimeCounter -= Time.deltaTime;
        }

		if (isTouchingFront)
		{
			canJump = true;
		}

		if (normMovementY > 0f)
		{
			interact = true;
		}
		else
		{
			interact = false;
		}
	}

    private void FixedUpdate()
	{
		if (jumping)
		{
			ChangeAnimationState(PLAYER_JUMP);
		}

		if (m_Grounded)
		{
			if (normMovementX != 0f)
			{
				ChangeAnimationState(PLAYER_RUN);
				CreateDust();
			}
			else
			{
				ChangeAnimationState(PLAYER_IDLE);
			}
		}
		else if (!jumping)
		{
			ChangeAnimationState(PLAYER_FALLING);
		}

		if (!knockbacked)
        {
			Move((normMovementX * movementSpeed * Time.fixedDeltaTime));
			CheckAxis();
		}

		bool wasGrounded = m_Grounded;
		m_Grounded = false;

		Collider2D[] colliders = Physics2D.OverlapCircleAll(m_GroundCheck.position, k_GroundedRadius, m_WhatIsGround);
		for (int i = 0; i < colliders.Length; i++)
		{
			if (colliders[i].gameObject != gameObject)
			{
				m_Grounded = true;
				canDash = true;

				if (!wasGrounded)
				{
					OnLandEvent.Invoke();
				}
			}
		}

		if (isDashing)
		{
			m_Rigidbody2D.velocity = transform.right * dashingDir * dashingVelocity;
			m_Rigidbody2D.constraints = RigidbodyConstraints2D.FreezePositionY;
			dashingTimer -= Time.fixedDeltaTime;

			if (dashingTimer <= 0)
			{
				isDashing = false;
				m_Rigidbody2D.constraints = RigidbodyConstraints2D.None;
				m_Rigidbody2D.constraints = RigidbodyConstraints2D.FreezeRotation;
			}
		}

		if (canWallJump)
		{
			isTouchingFront = Physics2D.OverlapCircle(frontWallCheck.position, k_GroundedRadius, m_WhatIsGround);
			if (isTouchingFront == true && !m_Grounded && normMovementX != 0f)
			{
				wallSliding = true;
			}
			else
			{
				wallSliding = false;
			}

			if (wallSliding == true)
			{
				m_Rigidbody2D.velocity = new Vector2(m_Rigidbody2D.velocity.x, Mathf.Clamp(m_Rigidbody2D.velocity.y, -wallSlidingSpeed, float.MaxValue));
			}

			if (wallJumping == true)
			{
				m_Rigidbody2D.velocity = new Vector2(xWallForce * -normMovementX, yWallForce);
				CreateDust();
			}
		}
	}


	void Dash()
    {
		isDashing = true;
		canDash = false;
		dashingTimer = startDashTimer;
	}

	public void Move(float move)
	{
		Vector3 targetVelocity = new Vector2(move * 10f, m_Rigidbody2D.velocity.y);
		m_Rigidbody2D.velocity = Vector3.SmoothDamp(m_Rigidbody2D.velocity, targetVelocity, ref m_Velocity, m_MovementSmoothing);
	}

	void CheckAxis()
    {
		if (normMovementX > 0f)
		{
			transform.localScale = new Vector2(1f, 1f);
		}

		if (normMovementX < 0f)
		{
			transform.localScale = new Vector2(-1, 1f);
		}

		if (normMovementY > 0f)
		{
			attackAnchor.localRotation = Quaternion.Euler(0, 0, 90);
		}
        else
        {
			if(dashingDir != 0)
            {
				attackAnchor.localRotation = Quaternion.Euler(0, 0, 0);
				attackRange = 1.7f;
			}
		}

		if (normMovementY < -0.2f && !m_Grounded)
		{
			attackAnchor.localRotation = Quaternion.Euler(0, 0, -90);
		}

		if (normMovementX == 0f && normMovementY == 0f && !m_Grounded)
		{
			attackPos.localPosition = new Vector2(0f, 0f);
			attackRange = 4f;
		}
		else
		{
			attackPos.localPosition = new Vector2(2.33f, 0f);
			attackRange = 1.7f;
		}
	}

	void SetWallJumpingToFalse()
    {
		wallJumping = false;
    }

	public void Knockback(Transform t)
    {
		Vector2 dir = transform.position - t.position;
		m_Rigidbody2D.velocity = dir.normalized * knockbackVel;
		knockbacked = true;
		StartCoroutine(ResetKnockback());
    }
	
	public void Attack()
	{
		Collider2D[] enemiesToDamage = Physics2D.OverlapCircleAll(attackPos.position, attackRange, whatIsEnemies);
		foreach (Collider2D enemy in enemiesToDamage)
		{
			enemy.GetComponent<EnemyHealth>().TakeDamage(attackDamage, transform);

			if (normMovementY < -0.5f && !m_Grounded)
			{
				m_Rigidbody2D.velocity = Vector2.up * m_JumpForce;
			}
		}
	}

	public void OnLanding()
    {
		canDash = true;
		m_Grounded = true;
		CreateDust();
		jumping = false;
	}

	IEnumerator FuryModeRoutine()
    {
		playerHealth.lives -= 2;
		playerHealth.healthBar.DrawHearts();
		canFuryMode = false;
		attackDamage = 2.5f;
		particle.SetActive(true);
		yield return new WaitForSeconds(10f);
		particle.SetActive(false);
		attackDamage = 1.6f;
		canFuryMode = true;
    }

	IEnumerator ResetKnockback()
    {
		yield return new WaitForSeconds(knockbackTimer * 2f);
		m_Rigidbody2D.constraints = RigidbodyConstraints2D.FreezePosition;
		yield return new WaitForSeconds(knockbackTimer * 1.5f);
		m_Rigidbody2D.constraints = RigidbodyConstraints2D.None;
		m_Rigidbody2D.constraints = RigidbodyConstraints2D.FreezeRotation;
		knockbacked = false;
    }

	private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("DoubleJump"))
        {
			canDoubleJump = true;
			Destroy(collision.gameObject);
        }
		else if (collision.gameObject.CompareTag("Dash"))
		{
			activeDash = true;
			Destroy(collision.gameObject);
		}
		else if (collision.gameObject.CompareTag("WallJump"))
		{
			canWallJump = true;
			Destroy(collision.gameObject);
		}
	}

    void CreateDust()
    {
		dustParticles.Play();
    }

	public void ChangeAnimationState(string newState)
	{
		if (currentState == newState) return;

		anim.Play(newState);

		currentState = newState;
    }

	public void Footstep()
    {
		FindObjectOfType<AudioManager>().Play("Footstep");
    }
	

	void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(attackPos.position, attackRange);
	}

    public object SaveState()
    {
		return new SaveData()
		{
			canDoubleJump = this.canDoubleJump,
			activeDash = this.activeDash,
			canWallJump = this.canWallJump
		};
	}

    public void LoadState(object state)
    {
		var saveData = (SaveData)state;
		canDoubleJump = saveData.canDoubleJump;
		activeDash = saveData.activeDash;
		canWallJump = saveData.canWallJump;
	}

	[Serializable]
	public struct SaveData
    {
		public bool canDoubleJump;
		public bool activeDash;
		public bool canWallJump;
	}
}