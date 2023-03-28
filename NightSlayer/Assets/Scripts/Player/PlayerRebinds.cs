using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerRebinds : MonoBehaviour
{
    public InputActionAsset asset;

    void OnEnable()
    {
        var rebinds = PlayerPrefs.GetString("rebinds");
        if (!string.IsNullOrEmpty(rebinds))
        {
            asset.LoadBindingOverridesFromJson(rebinds);
        }
    }

    void OnDisable()
    {
        var rebinds = PlayerPrefs.GetString("rebinds");
        if (!string.IsNullOrEmpty(rebinds))
        {
            asset.LoadBindingOverridesFromJson(rebinds);
        }
    }

    public void SaveRebinds()
    {
        var rebinds = asset.SaveBindingOverridesAsJson();
        PlayerPrefs.SetString("rebinds", rebinds);
    }
    
    public void LoadRebinds()
    {
        var rebinds = PlayerPrefs.GetString("rebinds");
        if (!string.IsNullOrEmpty(rebinds))
        {
            asset.LoadBindingOverridesFromJson(rebinds);
        }
    }
}