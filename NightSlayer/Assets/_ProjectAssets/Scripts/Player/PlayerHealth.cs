using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private float _currentHealth;
    [SerializeField] private float _maxHealth;
    [SerializeField] SpriteRenderer _spriteRenderer;
    [SerializeField] Animator anim;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Image healthBar;
    [SerializeField] private HealthbarAnim healthBarAnim;
    [SerializeField] GameObject playerDummy;
    
    bool invincible;

    public void Start()
    {
        _currentHealth = _maxHealth;
        _spriteRenderer.color = Color.white;
        invincible = false;

        healthBar.fillAmount = _currentHealth / _maxHealth;
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
        if(invincible == false)
        {
            //Knockback(attackDetails);
            anim.SetTrigger("Hurt");
            _currentHealth -= attackDetails.damageAmount;
            healthBar.fillAmount = _currentHealth / _maxHealth;
            StartCoroutine(DamageBlink());
            CinemachineShake.Instance.ShakeCamera(0.8f, 0.1f);
            HitStopController.Instance.Stop(0.1f);
            healthBarAnim.ShakeIt();
            if (CurrentHealth <= 0)
            {
                Instantiate(playerDummy, this.gameObject.transform.position, Quaternion.identity);
                CinemachineShake.Instance.ShakeCamera(2f, 0.5f);
                Destroy(this.gameObject, 0.25f);
            }
        }
    }

    public void GainHealth(float gainAmount)
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
        Color color = _spriteRenderer.color;
        for (int i = 0; i < 3; i++)
        {
            color.a = 1;
            yield return new WaitForSeconds(0.15f);
            color.a = 0;
            yield return new WaitForSeconds(0.15f);
        }
        color.a = 1;
        invincible = false;
    }
}