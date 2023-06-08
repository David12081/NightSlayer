using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using UnityEngine.SocialPlatforms;

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

    Transform target;
    [Header("Pathfinding")]
    [SerializeField] float speed = 200f;
    [SerializeField] float nextWaypointDist = 3f;

    Path path;
    int currentWayPoint = 0;
    bool reachedEndOfPath = false;

    Seeker seeker;
    [Header("Other")]
    [SerializeField] Rigidbody2D rb;
    [SerializeField] SpriteRenderer spriteRenderer;

    private void Start()
    {
        seeker = GetComponent<Seeker>();
        target = GameObject.Find("Player(Clone)").gameObject.transform;
        _currentHealth = _maxHealth;
        originalMaterial = spriteRenderer.material;

        InvokeRepeating("UpdatePath", 0f, 0.5f);
    }

    void UpdatePath()
    {
        if(seeker.IsDone())
        {
            seeker.StartPath(rb.position, target.position, OnPathComplete);
        }
    }

    void OnPathComplete(Path p)
    {
        if(!p.error)
        {
            path = p;
            currentWayPoint = 0;
        }
    }

    private void FixedUpdate()
    {
        if(path == null)
            return;

        if(currentWayPoint >= path.vectorPath.Count)
        {
            reachedEndOfPath = true;
            return;
        }
        else
        {
            reachedEndOfPath = false;
        }

        Vector2 direction = ((Vector2)path.vectorPath[currentWayPoint] - rb.position).normalized;
        Vector2 force = direction * speed * Time.deltaTime;

        rb.AddForce(force);

        float distance = Vector2.Distance(rb.position, path.vectorPath[currentWayPoint]);

        if(distance < nextWaypointDist)
        {
            currentWayPoint++;
        }

        if(force.x >= 0.01f)
        {
            spriteRenderer.flipX = false;
        }
        else if(force.x <= -0.01f)
        {
            spriteRenderer.flipX = true;
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