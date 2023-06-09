using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingEnemy : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private float _currentHealth;
    [SerializeField] private float _maxHealth;
    private Coroutine flashRoutine;
    private Material originalMaterial;
    [SerializeField] Material flashMaterial;
    [SerializeField] float flashDuration;
    [SerializeField] GameObject hitParticle;
    [SerializeField] GameObject deathBloodParticle;
    [SerializeField] int minScore;
    [SerializeField] int maxScore;

    [Header("Pathfinding")]
    [SerializeField] float speed;
    [SerializeField] float stoppingDistance;
    [SerializeField] float retreatDistance;
    Transform player;

    [Header("Attacking")]
    [SerializeField] float startTimeBtwShots;
    [SerializeField] GameObject projectile;
    float timeBtwShots;

    [Header("Other")]
    [SerializeField] Rigidbody2D rb;
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] Animator anim;

    private void Start()
    {
        player = GameObject.Find("Player(Clone)").gameObject.transform;
        _currentHealth = _maxHealth;
        originalMaterial = spriteRenderer.material;

        timeBtwShots = startTimeBtwShots;
    }

    private void Update()
    {
        if(player != null)
        {
            if (Vector2.Distance(transform.position, player.position) > stoppingDistance)
            {
                transform.position = Vector2.MoveTowards(transform.position, player.position, speed * Time.deltaTime);
            }
            else if (Vector2.Distance(transform.position, player.position) < stoppingDistance && Vector2.Distance(transform.position, player.position) > retreatDistance)
            {
                transform.position = this.transform.position;
            }
            else if (Vector2.Distance(transform.position, player.position) < retreatDistance)
            {
                transform.position = Vector2.MoveTowards(transform.position, player.position, -speed * Time.deltaTime);
            }

            if (transform.position.x - player.position.x < -0.01f)
            {
                spriteRenderer.flipX = false;
            }
            else if (transform.position.x - player.position.x > 0.01f)
            {
                spriteRenderer.flipX = true;
            }

            if (timeBtwShots <= 0)
            {
                anim.SetTrigger("Shoot");
                timeBtwShots = startTimeBtwShots;
            }
            else
            {
                timeBtwShots -= Time.deltaTime;
            }
        }
        else
        {
            return;
        }
    }

    public void Shoot()
    {
        Instantiate(projectile, transform.position, Quaternion.identity);
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
        Vector2 dir = rb.transform.position - attackDetails.position;
        Vector2 impulse = new Vector2(dir.x * attackDetails.knockbackForceX, attackDetails.knockbackForceY);
        rb.velocity = impulse;
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
}