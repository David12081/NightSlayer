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

    [SerializeField] List<GameObject> doors;
    [SerializeField] GameObject aStar;

    private void Start()
    {
        if(aStar == null)
        {
            return;
        }
        OpenDoors();
        aStar.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (playerOnPlatform)
        {
            _collider.enabled = false;
            //FindObjectOfType<AudioManager>().ChangeMusic(audioClip);
            CloseDoors();
            if(aStar != null)
            {
                aStar.gameObject.SetActive(true);
            }

            SpawnEnemies();
        }

        GameObject[] enemiesAlive = GameObject.FindGameObjectsWithTag("Enemy");
        if(enemiesAlive.Length >= 0)
        {
            OpenDoors();
            if (aStar != null)
            {
                aStar.gameObject.SetActive(false);
            }
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

    void CloseDoors()
    {
        for (int i = 0; i < doors.Count; i++)
        {
            doors[i].gameObject.SetActive(true);
        }
    }

    void OpenDoors()
    {
        for (int i = 0; i < doors.Count; i++)
        {
            doors[i].gameObject.SetActive(false);
        }
    }
}