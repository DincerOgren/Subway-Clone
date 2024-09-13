using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] Canvas pauseMenu;


    bool isPaused = false;


    Health health;

    private void Awake()
    {
        health = GetComponent<Health>();
    }
    // Update is called once per frame
    void Update()
    {
        if (!GameStarter.Instance.IsGameStarted()) return;
        if (health.IsPlayerDead()) return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                isPaused = false;
                pauseMenu.gameObject.SetActive(false);
                Time.timeScale = 1;
            }
            else
            {
                isPaused = true;
                pauseMenu.gameObject.SetActive(true);
                Time.timeScale = 0;

            }
        }
    }

    public void ResumeButton()
    {
        pauseMenu.gameObject.SetActive(false);
        isPaused = false;
        Time.timeScale = 1;
    }

    public void QuitButton()
    {
        Application.Quit();
    }
}
