using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public float life = 10f;
    private Rigidbody2D m_Rigidbody2D;
    [SerializeField] float knockbackVel;
    [SerializeField] float knockbackTimer;
    [HideInInspector] public bool knockbacked;

    private void Start()
    {
        m_Rigidbody2D = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (life <= 0)
        {
            Destroy(gameObject);
        }
    }

    public void TakeDamage(float damage, Transform t)
    {
        StartCoroutine(Damage(damage));
        Knockback(t);
    }

    IEnumerator Damage(float damage)
    {
        life -= damage;
        this.gameObject.GetComponent<SpriteRenderer>().color = Color.black;
        yield return new WaitForSeconds(0.1f);
        this.gameObject.GetComponent<SpriteRenderer>().color = Color.red;
        yield return new WaitForSeconds(0.1f);
    }

    void Knockback(Transform t)
    {
        Vector2 dir = transform.position - t.position;
        m_Rigidbody2D.velocity = dir.normalized * knockbackVel;
        knockbacked = true;
        StartCoroutine(ResetKnockback());
    }

    IEnumerator ResetKnockback()
    {
        yield return new WaitForSeconds(knockbackTimer * 2f);
        m_Rigidbody2D.constraints = RigidbodyConstraints2D.FreezePosition;
        yield return new WaitForSeconds(knockbackTimer * 1.5f);
        m_Rigidbody2D.constraints = RigidbodyConstraints2D.None;
        m_Rigidbody2D.constraints = RigidbodyConstraints2D.FreezeRotation;
        knockbacked = false;
    }
}