using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class JumperEnemy : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private float _currentHealth;
    [SerializeField] private float _maxHealth;
    private Coroutine flashRoutine;
    private Material originalMaterial;
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] Material flashMaterial;
    [SerializeField] float flashDuration;
    [SerializeField] GameObject hitParticle;
    [SerializeField] GameObject deathBloodParticle;
    [SerializeField] int minScore;
    [SerializeField] int maxScore;

    [Header("Attack")]
    [SerializeField] Transform attackPosition;
    [SerializeField] float attackRadius;
    [SerializeField] LayerMask whatIsPlayer;
    [SerializeField] float damageAmount;
    [SerializeField] int knockbackForceX;
    [SerializeField] int KnockbackForceY;
    AttackDetails attackDetails;

    [Header("For Patrolling")]
    [SerializeField] float moveSpeed;
    private float moveDirection = 1;
    private bool facingRight = true;
    [SerializeField] Transform groundCheckPoint;
    [SerializeField] Transform wallCheckPoint;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] float circleRadius;
    private bool checkingGround;
    private bool checkingWall;

    [Header("For JumpAttacking")]
    [SerializeField] float jumpHeight;
    Transform player;
    [SerializeField] Transform groundCheck;
    [SerializeField] Vector2 boxSize;
    private bool isGrounded;

    [Header("For SeeingPlayer")]
    [SerializeField] Vector2 lineOfSite;
    [SerializeField] LayerMask playerLayer;
    private bool canSeePlayer;

    [Header("Other")]
    [SerializeField] Animator enemyAnim;
    [SerializeField] Rigidbody2D enemyRB;

    private void Start()
    {
        player = GameObject.Find("Player(Clone)").gameObject.transform;
        _currentHealth = _maxHealth;
        originalMaterial = spriteRenderer.material;

        attackDetails.position = this.gameObject.transform.position;
        attackDetails.damageAmount = damageAmount;
        attackDetails.knockbackForceX = knockbackForceX;
        attackDetails.knockbackForceY = KnockbackForceY;
    }

    void FixedUpdate()
    {
        checkingGround = Physics2D.OverlapCircle(groundCheckPoint.position, circleRadius, groundLayer);
        checkingWall = Physics2D.OverlapCircle(wallCheckPoint.position, circleRadius, groundLayer);
        isGrounded = Physics2D.OverlapBox(groundCheck.position, boxSize, 0, groundLayer);
        canSeePlayer = Physics2D.OverlapBox(transform.position, lineOfSite, 0, playerLayer);
        AnimationController();
        if (!canSeePlayer && isGrounded)
        {
            Patrolling();
        }
    }

    void Patrolling()
    {
        if (!checkingGround || checkingWall)
        {
            if (facingRight)
            {
                Flip();
            }
            else if (!facingRight)
            {
                Flip();
            }
        }
        enemyRB.velocity = new Vector2(moveSpeed * moveDirection, enemyRB.velocity.y);
    }

    void JumpAttack()
    {
        if(player != null)
        {
            float distanceFromPlayer = player.position.x - transform.position.x;

            if (isGrounded)
            {
                enemyRB.AddForce(new Vector2(distanceFromPlayer + (5f * moveDirection), jumpHeight), ForceMode2D.Impulse);
            }
        }
    }

    void FlipTowardsPlayer()
    {
        if(player != null)
        {
            float playerPosition = player.position.x - transform.position.x;
            if (playerPosition < 0 && facingRight)
            {
                Flip();
            }
            else if (playerPosition > 0 && !facingRight)
            {
                Flip();
            }
        }
    }

    void Flip()
    {
        moveDirection *= -1;
        facingRight = !facingRight;
        transform.Rotate(0, 180, 0);
    }

    void AnimationController()
    {
        enemyAnim.SetBool("canSeePlayer", canSeePlayer);
        enemyAnim.SetBool("isGrounded", isGrounded);
    }

    public void TriggerAttack()
    {
        Collider2D[] detectedObjects = Physics2D.OverlapCircleAll(attackPosition.position, attackRadius, whatIsPlayer);

        foreach (Collider2D collider in detectedObjects)
        {
            collider.transform.SendMessage("Damage", attackDetails);
            collider.transform.SendMessage("Knockback", attackDetails);
        }
    }

    public void Damage(AttackDetails attackDetails)
    {
        _currentHealth -= attackDetails.damageAmount;

        Knockback(attackDetails);
        Flash();

        Instantiate(hitParticle, transform.position, Quaternion.Euler(0f, 0f, Random.Range(0f, 360f)));

        if (_currentHealth <= 0)
        {
            int randomScore = Random.Range(minScore, maxScore);
            int gameScore = PlayerPrefs.GetInt("GameScore");
            PlayerPrefs.SetInt("GameScore", gameScore + randomScore);
            Score.Instance.UpdateText();

            GameObject.Instantiate(deathBloodParticle, this.transform.position, deathBloodParticle.transform.rotation);
            Destroy(this.gameObject);
        }
    }

    public void Knockback(AttackDetails attackDetails)
    {
        Vector2 dir = enemyRB.transform.position - attackDetails.position;
        Vector2 impulse = new Vector2(dir.x * attackDetails.knockbackForceX, attackDetails.knockbackForceY);
        enemyRB.velocity = impulse;
    }

    public void Flash()
    {
        if (flashRoutine != null)
        {
            StopCoroutine(flashRoutine);
        }
        flashRoutine = StartCoroutine(FlashRoutine());
    }

    IEnumerator FlashRoutine()
    {
        spriteRenderer.material = flashMaterial;
        yield return new WaitForSeconds(flashDuration);
        spriteRenderer.material = originalMaterial;
        flashRoutine = null;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(groundCheckPoint.position, circleRadius);
        Gizmos.DrawWireSphere(wallCheckPoint.position, circleRadius);

        Gizmos.color = Color.green;
        Gizmos.DrawCube(groundCheck.position, boxSize);

        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, lineOfSite);

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(attackPosition.position, attackRadius);
    }
}