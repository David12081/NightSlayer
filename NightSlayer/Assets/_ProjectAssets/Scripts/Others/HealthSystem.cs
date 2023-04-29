using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class HealthSystem : MonoBehaviour
{
    [Header("Health Variables")]
    [SerializeField] private Rigidbody2D m_Rigidbody2D;
    [SerializeField] private int m_currentHealth;
    [SerializeField] private int m_maxHealth;
    [SerializeField] private UnityEvent OnZeroHealth;

    [Header("Damage Blink Variables")]
    [SerializeField] private Material flashMaterial;
    [SerializeField] float flashDuration;
    [SerializeField] SpriteRenderer spriteRenderer;
    private Material originalMaterial;
    private Coroutine flashRoutine;

    public void Awake()
    {
        m_currentHealth = m_maxHealth;

        spriteRenderer = GetComponent<SpriteRenderer>();
        originalMaterial = spriteRenderer.material;
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

    public void DestroyObject()
    {
        Destroy(this.gameObject);
    }

    public void Knockback(Transform t, float knockbackForceX, float knockbackForceY)
    {
        Vector2 dir = transform.position - t.position;
        m_Rigidbody2D.velocity = new Vector2(dir.x * knockbackForceX, knockbackForceY);
    }

    public void Flash()
    {
        if(flashRoutine != null)
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