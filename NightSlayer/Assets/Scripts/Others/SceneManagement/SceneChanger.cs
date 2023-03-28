using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    [SerializeField] SceneConnection connection;
    [SerializeField] string targetSceneName;
    [SerializeField] Transform spawnPoint;
    public SceneFade sceneFade;
    private PlayerController playerController;

    private void Awake()
    {
        playerController = FindObjectOfType<PlayerController>();
        if (playerController == null)
        {
            return;
        }

        if (connection == SceneConnection.activeConnection)
        {
            playerController.transform.position = spawnPoint.position;
        }
        else
        {
            return;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        var player = collision.collider.GetComponent<PlayerController>();
        if (player != null)
        {
            SceneConnection.activeConnection = connection;
            sceneFade.FadeTo(targetSceneName);
        }
    }
}