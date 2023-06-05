using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRoom : MonoBehaviour
{
    [SerializeField] private Collider2D _collider;
    [SerializeField] private AudioClip audioClip;
    private bool playerOnPlatform = false;
    private PlayerScript playerScript;

    private void Update()
    {
        if (playerOnPlatform)
        {
            _collider.enabled = false;
            FindObjectOfType<AudioManager>().ChangeMusic(audioClip);
        }
    }

    void SetPlayerOnPlatform(Collider2D other, bool value)
    {
        var player = other.gameObject.GetComponent<PlayerScript>();
        if (player != null)
        {
            playerScript = player;
            playerOnPlatform = value;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        SetPlayerOnPlatform(collision, true);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        SetPlayerOnPlatform(collision, false);
    }
}