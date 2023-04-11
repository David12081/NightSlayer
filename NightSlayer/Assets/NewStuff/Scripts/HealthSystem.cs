using UnityEngine;
using UnityEngine.Events;

public class HealthSystem : MonoBehaviour
{
    [SerializeField] private Rigidbody2D m_Rigidbody2D;
    [SerializeField] private int m_currentHealth;
    [SerializeField] private int m_maxHealth;
    [SerializeField] private UnityEvent OnZeroHealth;

    public void Awake()
    {
        m_currentHealth = m_maxHealth;
    }

    public int CurrentHealth
    {
        get => m_currentHealth;
        set => m_currentHealth = value;
    }

    public int MaxHealth
    {
        get => m_maxHealth;
        set => m_maxHealth = value;
    }

    public void ReceiveDamage(int damageAmount)
    {
        m_currentHealth -= damageAmount;
        //StartCoroutine(DamageBlink());
        if (CurrentHealth <= 0)
        {
            OnZeroHealth?.Invoke();
        }
    }

    public void GainHealth(int gainAmount)
    {
        m_currentHealth += gainAmount;
        m_currentHealth = Mathf.Clamp(m_currentHealth, 0, m_maxHealth);
    }

    //IEnumerator DamageBlink()
    //{
    //    for (int i = 0; i < 5; i++)
    //    {
    //        _material.color = Color.white;
    //        yield return new WaitForSeconds(0.15f);
    //        _material.color = Color.red;
    //        yield return new WaitForSeconds(0.15f);
    //    }
    //    _material.color = Color.white;
    //}

    public void DestroyObject()
    {
        Destroy(this.gameObject);
    }

    public void Knockback(Transform t, float knockbackForceX, float knockbackForceY)
    {
        Vector2 dir = transform.position - t.position;
        m_Rigidbody2D.velocity = new Vector2(dir.x * knockbackForceX, knockbackForceY);
        //StartCoroutine(ResetKnockback());
    }
}