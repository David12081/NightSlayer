using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRoom : MonoBehaviour
{
    [SerializeField] private Collider2D _collider;
    [SerializeField] private AudioClip audioClip;
    private bool playerOnPlatform = false;

    [SerializeField] private List<GameObject> enemies;
    [SerializeField] private Transform[] spawnPosition;

    private void Update()
    {
        if (playerOnPlatform)
        {
            _collider.enabled = false;
            //FindObjectOfType<AudioManager>().ChangeMusic(audioClip);

            SpawnEnemies();
        }
    }

    void SetPlayerOnPlatform(Collider2D other, bool value)
    {
        var player = other.gameObject.GetComponent<PlayerScript>();
        if (player != null)
        {
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

    void SpawnEnemies()
    {
        for (int i = 0; i < enemies.Count; i++)
        {
            Instantiate(enemies[i], spawnPosition[i].position, Quaternion.identity);
        }
    }
}