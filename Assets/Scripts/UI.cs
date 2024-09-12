using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI scoreText;
    [SerializeField] TextMeshProUGUI highScoreText;
    [SerializeField] TextMeshProUGUI coinText;

    [SerializeField] bool isUsingForButtonMethods = false;

    private void Update()
    {
        if (isUsingForButtonMethods) { return; }
        scoreText.text = ScoreManager.instance.GetScore().ToString();
        highScoreText.text = ScoreManager.instance.GetHighScore().ToString();
        coinText.text = PlayerCollectibleManager.instance.GetCurrentCoins().ToString();

    }

    public void RestartButton()
    {
        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        //   GameStarter.instance.RestartGame();
        GameManager.Instance.DeleteAllChilds();
        GameManager.Instance.ResetPlayerLocation();


        //Spawn Start plane 
        // Change UI's 
        //Change HEalth states 
        // Reset Chasing Enemy;

    }

    public void QuitButton()
    {
        Application.Quit();
    }
}
