using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallThroughPlatform : MonoBehaviour
{
    [SerializeField] private Collider2D _collider;
    private Collider2D _playerCollider;
    private bool playerOnPlatform;
    private PlayerScript playerScript;

    private void Update()
    {
        if (playerOnPlatform && playerScript.InputY <= -0.5f)
        {
            Physics2D.IgnoreCollision(_collider, _playerCollider, true);
            StartCoroutine(EnableCollision());
        }
    }

    IEnumerator EnableCollision()
    {
        yield return new WaitForSeconds(0.5f);
        Physics2D.IgnoreCollision(_collider, _playerCollider, false);
    }

    void SetPlayerOnPlatform(Collision2D other, bool value)
    {
        var player = other.gameObject.GetComponent<PlayerScript>();
        if(player != null)
        {
            playerScript = player;
            playerOnPlatform = value;
            _playerCollider = playerScript.GetComponent<Collider2D>();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        SetPlayerOnPlatform(collision, true);
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        SetPlayerOnPlatform(collision, false);
    }
}