using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    public AudioMixer audioMixer;

    public Dropdown resolutionsDropdown, qualityDropdown, framerateDropdown;
    public Slider globalVolumeSlider, musicVolumeSlider, effectsVolumeSlider;

    private bool fullscreen = true;

    Resolution[] resolutions;

    private void Start()
    {
        SetQuality(PlayerPrefs.GetInt("qualityIndex"));
        SetFramerate(PlayerPrefs.GetInt("framerateIndex"));
        SetGlobalVolume(PlayerPrefs.GetFloat("globalVolume"));
        SetMusicVolume(PlayerPrefs.GetFloat("musicVolume"));
        SetEffectsVolume(PlayerPrefs.GetFloat("effectsVolume"));

        if (resolutionsDropdown == null)
        {
            return;
        }

        resolutions = Screen.resolutions;

        resolutionsDropdown.ClearOptions();

        List<string> options = new List<string>();

        int currentResolutionIndex = 0;

        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            options.Add(option);

            if (resolutions[i].width == Screen.currentResolution.width && resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }

        resolutionsDropdown.AddOptions(options);
        resolutionsDropdown.value = currentResolutionIndex;
        resolutionsDropdown.RefreshShownValue();

        SetResolution(PlayerPrefs.GetInt("resolutionIndex"));
    }

    private void Update()
    {
        if (resolutionsDropdown == null || globalVolumeSlider == null || qualityDropdown == null || framerateDropdown == null
            || musicVolumeSlider == null || effectsVolumeSlider == null)
        {
            return;
        }
        else
        {
            SetGlobalVolume(PlayerPrefs.GetFloat("globalVolume"));
            globalVolumeSlider.value = PlayerPrefs.GetFloat("globalVolume");

            SetMusicVolume(PlayerPrefs.GetFloat("musicVolume"));
            musicVolumeSlider.value = PlayerPrefs.GetFloat("musicVolume");

            SetEffectsVolume(PlayerPrefs.GetFloat("effectsVolume"));
            effectsVolumeSlider.value = PlayerPrefs.GetFloat("effectsVolume");

            SetQuality(PlayerPrefs.GetInt("qualityIndex"));
            qualityDropdown.value = PlayerPrefs.GetInt("qualityIndex");

            SetResolution(PlayerPrefs.GetInt("resolutionIndex"));
            resolutionsDropdown.value = PlayerPrefs.GetInt("resolutionIndex");

            SetFramerate(PlayerPrefs.GetInt("framerateIndex"));
            framerateDropdown.value = PlayerPrefs.GetInt("framerateIndex");
        }
    }

    public void SetGlobalVolume(float volume)
    {
        audioMixer.SetFloat("globalVolume", volume);
        PlayerPrefs.SetFloat("globalVolume", volume);
    }

    public void SetMusicVolume(float volume)
    {
        audioMixer.SetFloat("musicVolume", volume);
        PlayerPrefs.SetFloat("musicVolume", volume);
    }

    public void SetEffectsVolume(float volume)
    {
        audioMixer.SetFloat("effectsVolume", volume);
        PlayerPrefs.SetFloat("effectsVolume", volume);
    }

    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
        PlayerPrefs.SetInt("qualityIndex", qualityIndex);
    }

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
        fullscreen = isFullscreen;
    }

    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, fullscreen);
        PlayerPrefs.SetInt("resolutionIndex", resolutionIndex);
    }

    public void SetFramerate(int framerateIndex)
    {
        PlayerPrefs.SetInt("framerateIndex", framerateIndex);
        switch (framerateIndex)
        {
            case 0:
                Application.targetFrameRate = 30;
                break;

            case 1:
                Application.targetFrameRate = 60;
                break;

            case 2:
                Application.targetFrameRate = -1;
                break;
        }
    }

    public void PlayButtonSound()
    {
        FindObjectOfType<AudioManager>().Play("Button");
    }

    public void ResetHighScore()
    {
        PlayerPrefs.SetInt("HighScore", 0);
        PlayerPrefs.SetInt("GameScore", 0);
    }
}