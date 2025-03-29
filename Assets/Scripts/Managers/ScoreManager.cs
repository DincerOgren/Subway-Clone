using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class ScoreManager : MonoBehaviour
{
    public static ScoreManager instance;


    public float currentScore = 0;
    public float highScore = 0;
    public float scoreAddedPerTick = 2;
    public float scoreTickRate = .1f;
    public float scoreTimer = 0;

    [SerializeField] TextMeshProUGUI scoreText;

    [Header("Power Up")]
    [SerializeField] float scoreMultiplier = 5f;
    PowerUp scoreMultiplierData;


    Health playerRef;

    private void Awake()
    {
        playerRef = GameObject.FindWithTag("Player").GetComponent<Health>();
    }
    private void Start()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        scoreMultiplierData = PowerUpManager.instance.GetPowerUpData(PowerUpType.ScoreMultiplier);
        highScore = PlayerPrefs.GetFloat("HighScore", currentScore);
        // Add Save System

        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        if (!GameStarter.Instance.IsGameStarted()) return;
        if (playerRef.IsPlayerDead()) return;

        if (scoreTimer > scoreTickRate)
        {
            scoreTimer = 0;

            if (scoreMultiplierData.IsActive)
            {
                currentScore += scoreAddedPerTick * scoreMultiplier;
            }
            else
            {
                currentScore += scoreAddedPerTick;
            }

        }

        scoreText.text = "x" + scoreMultiplier.ToString();

        if (currentScore > highScore)
        {
            highScore = currentScore;
        }

        
        scoreTimer += Time.deltaTime;
    }

    public void SaveScore()
    {
        float tempHigh = PlayerPrefs.GetFloat("HighScore");
        if (highScore > tempHigh)
        {
            PlayerPrefs.SetFloat("HighScore", currentScore);

        }
    }

    public float GetScore() => currentScore;
    public float GetHighScore() => highScore;

    public void ResetScore()
    {
        scoreTimer = 0;
        currentScore = 0;
    }
    public float AddScore(float v) => currentScore += v;
}
