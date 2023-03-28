using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour, ISaveable
{
    public int lives;
    public int livesLimit;
    private SpriteRenderer sprite;
    private bool invincible;
    [SerializeField] Color spriteColor;
    public HealthBar healthBar;
    private PlayerController playerController;

    private void Awake()
    {
        lives = livesLimit;
        sprite = GetComponent<SpriteRenderer>();

        playerController = GetComponent<PlayerController>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            if(!invincible)
            {
                playerController.Knockback(collision.transform);
                StartCoroutine(Damage());
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Potion"))
        {
            if(lives >= livesLimit)
            {
                lives = livesLimit;
            }
            else
            {
                lives++;
                healthBar.DrawHearts();
            }
            Destroy(collision.gameObject);
        }

        else if(collision.gameObject.CompareTag("Heart"))
        {
            livesLimit += 2;
            lives = livesLimit;
            healthBar.DrawHearts();
            Destroy(collision.gameObject);
        }
    }

    IEnumerator Damage()
    {
        lives--;
        healthBar.DrawHearts();
        invincible = true;
        for (int i = 0; i < 5; i++)
        {
            sprite.color = Color.black;
            yield return new WaitForSeconds(0.15f);
            sprite.color = spriteColor;
            yield return new WaitForSeconds(0.15f);
        }
        invincible = false;
    }

    public object SaveState()
    {
        return new SaveData()
        {
            lives = this.lives,
            livesLimit = this.livesLimit
        };
    }

    public void LoadState(object state)
    {
        var saveData = (SaveData)state;
        lives = saveData.lives;
        livesLimit = saveData.livesLimit;
        healthBar.DrawHearts();
    }

    [Serializable]
    private struct SaveData
    {
        public int lives;
        public int livesLimit;
    }
}