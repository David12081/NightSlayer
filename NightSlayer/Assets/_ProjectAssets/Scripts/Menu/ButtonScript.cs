using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class ButtonScript : MonoBehaviour
{
    public string scene;
    public GameObject lastMenu, currentMenu;
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