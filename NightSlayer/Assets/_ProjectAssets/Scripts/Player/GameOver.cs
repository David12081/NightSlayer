using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{
    [SerializeField] GameObject gameOverCanvas;
    [SerializeField] GameObject playerPrefab;
    [SerializeField] GameObject timeline;

    private void Start()
    {
        if(timeline == null )
        {
            return;
        }
    }

    void SpawnGameOver()
    {
        Instantiate(gameOverCanvas);
    }

    void PlayMusic()
    {
        FindObjectOfType<AudioManager>().Play("GameOver");
    }

    void LoadMenu()
    {
        SceneManager.LoadScene("Menu");
    }

    void SpawnPlayer()
    {
        Instantiate(playerPrefab, timeline.transform.position, Quaternion.identity);
    }

    void DestroyTimeline()
    {
        Destroy(timeline);
    }
}