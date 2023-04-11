using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using System.IO;

public class ButtonScript : MonoBehaviour
{
    public string scene;
    public GameObject lastMenu, currentMenu;
    public SaveLoadSystem saveLoadSystem;
    public GameObject menuCanvas, decisionCanvas;
    public GameObject player;

    private void Awake()
    {
        if(lastMenu == null)
        {
            return;
        }
        if (currentMenu == null)
        {
            return;
        }
        if(scene == null)
        {
            return;
        }
        if(player == null)
        {
            return;
        }

        menuCanvas.SetActive(true);
        decisionCanvas.SetActive(false);
    }

    public void GoToScene(string scene)
    {
        SceneManager.LoadScene(scene);
    }

    public void NewGameButton()
    {
        if(!File.Exists(saveLoadSystem.SavePath))
        {
            saveLoadSystem.DeleteSaveFile();
            GoToScene("Test");
            PlayerPrefs.SetString("SavedLevel", "Test");
            AudioManager.instance.ChangeMusic(AudioManager.instance.clips[1]);
        }
        else
        {
            menuCanvas.SetActive(false);
            decisionCanvas.SetActive(true);
        }
    }

    public void LoadGameButton()
    {
        if (PlayerPrefs.HasKey("SavedLevel"))
        {
            AudioManager.instance.ChangeMusic(AudioManager.instance.clips[1]);
            string levelToLoad = PlayerPrefs.GetString("SavedLevel");
            SceneManager.LoadScene(levelToLoad);
        }
    }

    public void DestroyPlayer()
    {
        Destroy(player);
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void InputBackButton(InputAction.CallbackContext value)
    {
        if (value.performed)
        {
            SceneManager.LoadScene("Menu");
        }
    }

    public void InputBackButtonMenu(InputAction.CallbackContext value)
    {
        if(value.performed)
        {
            currentMenu.SetActive(false);
            lastMenu.SetActive(true);
        }
    }
}