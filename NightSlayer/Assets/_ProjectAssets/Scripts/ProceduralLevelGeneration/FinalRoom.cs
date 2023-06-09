using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinalRoom : MonoBehaviour
{
    [SerializeField] private Collider2D _collider;
    [SerializeField] private AudioClip audioClip;
    [SerializeField] private AudioClip audioClip1;
    private bool playerOnPlatform = false;

    [SerializeField] private List<GameObject> enemies;
    [SerializeField] private Transform[] spawnPosition;

    [SerializeField] List<GameObject> doors;

    [SerializeField] GameObject teleporter;

    bool enemiesSpawned;

    private void Awake()
    {
        OpenDoors();
        enemiesSpawned = false;
        teleporter.SetActive(false);
    }

    private void Update()
    {
        if (playerOnPlatform)
        {
            CloseDoors();
            _collider.enabled = false;
            FindObjectOfType<AudioManager>().ChangeMusic(audioClip);
            SpawnEnemies();
        }

        if(enemiesSpawned == true)
        {
            GameObject[] enemiesAlive = GameObject.FindGameObjectsWithTag("Enemy");
            if (enemiesAlive.Length <= 0)
            {
                OpenDoors();
                FindObjectOfType<AudioManager>().ChangeMusic(audioClip1);
                teleporter.SetActive(true);
                Destroy(this.gameObject);
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
        enemiesSpawned = true;
    }

    void CloseDoors()
    {
        for (int i = 0; i < doors.Count; i++)
        {
            doors[i].SetActive(true);
        }
    }

    void OpenDoors()
    {
        for (int i = 0; i < doors.Count; i++)
        {
            doors[i].SetActive(false);
        }
    }
}