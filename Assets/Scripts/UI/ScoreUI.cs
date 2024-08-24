using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreUI : MonoBehaviour
{
    public RectTransform backgroundPanel; 
    public TextMeshProUGUI scoreText;     
    public float score = 0;
    public float padding = 50f;           

    void Start()
    {
        UpdateScore(000000);
    }

    private void Update()
    {
        score = ScoreManager.instance.GetScore();
        UpdateScore(score);
    }
    
    private void UpdateScore(float newScore)
    {
        scoreText.text = ((int)newScore).ToString("D6");

        float preferredWidth = scoreText.preferredWidth;

        backgroundPanel.sizeDelta = new Vector2(preferredWidth + padding, backgroundPanel.sizeDelta.y);
    }
}
