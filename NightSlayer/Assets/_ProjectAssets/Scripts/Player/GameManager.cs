using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    private bool isPaused;
    [SerializeField] private PlayerInput playerInput;
    public PlayerRebinds playerRebinds;

    private string actionMapPlayerControls = "Player";
    private string actionMapMenuControls = "UI Navigation";
    public GameObject canvas, rebind, pauseTextCanvas, optionsCanvas, audioCanvas, keyboardCanvas, controllerCanvas;

    private void Awake()
    {
        isPaused = false;
        playerInput = GetComponent<PlayerInput>();
        playerRebinds.LoadRebinds();
    }

    public void OnTogglePause(InputAction.CallbackContext value)
    {
        if (value.started)
        {
            TogglePauseState();
        }
    }

    public void TogglePauseState()
    {
        isPaused = !isPaused;

        ToggleTimeScale();
        SwitchControlScheme();
        UpdateUI();
    }

    void ToggleTimeScale()
    {
        float newTimeScale = 0f;

        switch (isPaused)
        {
            case true:
                newTimeScale = 0f;
                break;

            case false:
                newTimeScale = 1f;
                break;
        }

        Time.timeScale = newTimeScale;
    }

    void UpdateUI()
    {
        switch (isPaused)
        {
            case true:
                canvas.SetActive(true);
                pauseTextCanvas.SetActive(true);
                rebind.SetActive(true);
                break;

            case false:
                canvas.SetActive(false);
                rebind.SetActive(false);
                optionsCanvas.SetActive(false);
                audioCanvas.SetActive(false);
                keyboardCanvas.SetActive(false);
                controllerCanvas.SetActive(false);
                break;
        }
    }

    void SwitchControlScheme()
    {
        switch (isPaused)
        {
            case true:
                EnablePauseMenuControls();
                break;

            case false:
                EnableGameplayControls();
                break;
        }
    }

    public void EnableGameplayControls()
    {
        playerInput.SwitchCurrentActionMap(actionMapPlayerControls);
    }

    public void EnablePauseMenuControls()
    {
        playerInput.SwitchCurrentActionMap(actionMapMenuControls);
    }
}