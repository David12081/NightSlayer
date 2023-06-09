using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Score : MonoBehaviour
{
    public static Score Instance { get; private set; }
    [SerializeField] TextMeshProUGUI scoreText;

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {
        scoreText.text = "Score: " + PlayerPrefs.GetInt("GameScore");
    }

    public void UpdateText()
    {
        PlayerPrefs.GetInt("GameScore");
        scoreText.text = "Score: " + PlayerPrefs.GetInt("GameScore");
    }
}