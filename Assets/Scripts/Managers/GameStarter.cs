using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class GameStarter : MonoBehaviour
{
    public static GameStarter Instance;
    [SerializeField] CinemachineVirtualCamera starterCamera;
    [SerializeField] CinemachineVirtualCamera followCamera;

    [SerializeField] GameObject inGameCanvas;
    [SerializeField] GameObject startCanvas;
    [SerializeField] GameObject endGameCanvas;

    public bool shouldStartGame;

    [SerializeField] TextMeshProUGUI highScore;

    bool isGameStarted = false;
    Transform playerRef;

    Vector3 playerAngles;
    Vector3 playerStartPos;
    private void Awake()
    {
        playerRef = GameObject.FindWithTag("Player").transform;
        followCamera.Priority = 1;

    }
    private void Start()
    {
        Instance = this;

        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(this.gameObject);


        playerStartPos = playerRef.position;
        playerAngles = playerRef.eulerAngles;
        highScore.text = PlayerPrefs.GetFloat("HighScore").ToString();

    }

    private void Update()
    {
        if (shouldStartGame && !isGameStarted)
        {
            StartGame();
        }
    }

    private void StartGame()
    {
        isGameStarted = true;
        playerRef.transform.eulerAngles = Vector3.zero;
        starterCamera.Priority = 1;
        followCamera.Priority = 10;
        startCanvas.SetActive(false);
        inGameCanvas.SetActive(true);
    }

    public void ChangeCamera()
    {

        starterCamera.Priority = 10;
        followCamera.Priority = 1;
    }
    public void RestartGame()
    {
        isGameStarted = false;
        shouldStartGame = false;
        playerRef.transform.eulerAngles = playerAngles;
        playerRef.transform.position = playerStartPos;
        startCanvas.SetActive(true);
        endGameCanvas.SetActive(false);
        inGameCanvas.SetActive(false);
        highScore.text = PlayerPrefs.GetFloat("HighScore").ToString();

    }

    public bool IsGameStarted() => isGameStarted;

    public void SetGameStarted(bool value) => isGameStarted = value;
}
