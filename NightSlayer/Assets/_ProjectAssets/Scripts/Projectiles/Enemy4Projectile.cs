using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy4Projectile : MonoBehaviour
{
    [Header("Other")]
    [SerializeField] float speed;
    [SerializeField] private LayerMask whatIsGround;
    [SerializeField] GameObject destroyedParticles;
    Transform player;
    Vector2 target;

    [Header("Damage")]
    [SerializeField] float projectileDamage;
    [SerializeField] int knockbackForceX;
    [SerializeField] int knockbackForceY;
    AttackDetails attackDetails;

    private void Start()
    {
        player = GameObject.Find("Player(Clone)").gameObject.transform;
        target = new Vector2(player.position.x, player.position.y);

        attackDetails.damageAmount = projectileDamage;
        attackDetails.position = gameObject.transform.position;
        attackDetails.knockbackForceX = knockbackForceX;
        attackDetails.knockbackForceY = knockbackForceY;
    }

    private void Update()
    {
        transform.position = Vector2.MoveTowards(transform.position, target, speed * Time.deltaTime);

        if(transform.position.x == target.x && transform.position.y == target.y)
        {
            DestroyProjectile();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            collision.transform.SendMessage("Damage", attackDetails);
            DestroyProjectile();
        }
        if(collision.gameObject.layer == whatIsGround)
        {
            DestroyProjectile();
        }
    }

    void DestroyProjectile()
    {
        Instantiate(destroyedParticles, gameObject.transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}