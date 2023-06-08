using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MenuHighScore : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI highScoreText;
    
    private void Start()
    {
        highScoreText.text = "High Score: " + PlayerPrefs.GetInt("HighScore", 0).ToString();

        if(PlayerPrefs.GetInt("GameScore") > PlayerPrefs.GetInt("HighScore", 0))
        {
            PlayerPrefs.SetInt("HighScore", PlayerPrefs.GetInt("GameScore"));
            highScoreText.text = "High Score: " + PlayerPrefs.GetInt("GameScore").ToString();
        }
    }
}