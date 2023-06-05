using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private float _currentHealth;
    [SerializeField] private float _maxHealth;
    [SerializeField] SpriteRenderer _spriteRenderer;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Image healthBar;
    [SerializeField] private HealthbarAnim healthBarAnim;
    [SerializeField] private UnityEvent OnZeroHealth;
    
    bool invincible;

    private AudioManager audioManager;

    public void Start()
    {
        _currentHealth = _maxHealth;
        _spriteRenderer.color = Color.white;
        invincible = false;

        healthBar.fillAmount = _currentHealth / _maxHealth;

        audioManager = FindObjectOfType<AudioManager>();
    }

    public float CurrentHealth
    {
        get => _currentHealth;
        set => _currentHealth = value;
    }

    public float MaxHealth
    {
        get => _maxHealth;
        set => _maxHealth = value;
    }

    public void Damage(AttackDetails attackDetails)
    {
        if(!invincible)
        {
            Knockback(attackDetails);
            _currentHealth -= attackDetails.damageAmount;
            healthBar.fillAmount = _currentHealth / _maxHealth;
            StartCoroutine(DamageBlink());
            CinemachineShake.Instance.ShakeCamera(0.8f, 0.1f);
            HitStopController.Instance.Stop(0.1f);
            healthBarAnim.ShakeIt();
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
        Vector2 impulse = new Vector2(dir.x * attackDetails.knockbackForceX, attackDetails.knockbackForceY);
        rb.velocity = impulse;
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