using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{
    void ChangeScene()
    {
        SceneManager.LoadScene("Menu");
    }
}