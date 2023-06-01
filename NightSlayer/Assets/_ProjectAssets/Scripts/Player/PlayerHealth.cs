using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private int _currentHealth;
    [SerializeField] private int _maxHealth;
    [SerializeField] SpriteRenderer _spriteRenderer;
    [SerializeField] private UnityEvent OnZeroHealth;
    [SerializeField] private Rigidbody2D rb;
    bool invincible;

    public void Start()
    {
        _currentHealth = _maxHealth;
        _spriteRenderer.color = Color.white;
        invincible = false;
    }

    public int CurrentHealth
    {
        get => _currentHealth;
        set => _currentHealth = value;
    }

    public int MaxHealth
    {
        get => _maxHealth;
        set => _maxHealth = value;
    }

    public void Damage(AttackDetails attackDetails)
    {
        if(!invincible)
        {
            _currentHealth -= attackDetails.damageAmount;
            StartCoroutine(DamageBlink());
            Knockback(attackDetails);
            if (CurrentHealth <= 0)
            {
                OnZeroHealth?.Invoke();
            }
        }
    }

    public void GainHealth(int gainAmount)
    {
        _currentHealth += gainAmount;
        _currentHealth = Mathf.Clamp(_currentHealth, 0, _maxHealth);
    }

    public void Knockback(AttackDetails attackDetails)
    {
        Vector2 dir = rb.transform.position - attackDetails.position;
        rb.velocity = new Vector2(dir.x * attackDetails.knockbackForceX, attackDetails.knockbackForceY);
    }

    IEnumerator DamageBlink()
    {
        invincible = true;
        for (int i = 0; i < 2; i++)
        {
            _spriteRenderer.color = Color.white;
            yield return new WaitForSeconds(0.15f);
            _spriteRenderer.color = Color.black;
            yield return new WaitForSeconds(0.15f);
        }
        _spriteRenderer.color = Color.white;
        invincible = false;
    }
}