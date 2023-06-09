using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{
    [SerializeField] GameObject gameOverCanvas;
    
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
}