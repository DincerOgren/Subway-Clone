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
    public static GameStarter instance;
    [SerializeField] CinemachineVirtualCamera starterCamera;
    [SerializeField] CinemachineVirtualCamera followCamera;

    [SerializeField] GameObject inGameCanvas;
    [SerializeField] GameObject startCanvas;
    [SerializeField] GameObject endGameCanvas;

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
        instance = this;
        playerStartPos = playerRef.position;
        playerAngles = playerRef.eulerAngles;
        highScore.text = PlayerPrefs.GetFloat("HighScore").ToString();

    }

    private void Update()
    {
        if (Input.anyKeyDown && !isGameStarted)
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


    public bool IsGameStarted() => isGameStarted;

    public void SetGameStarted(bool value) => isGameStarted = value;
}
