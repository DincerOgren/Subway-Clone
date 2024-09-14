using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missions : MonoBehaviour
{
    public static Missions Instance;

    public Mission trainHitMission;
    private void Start()
    {

        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }


    public void AddToMission(int value)
    {
        value++;
    }

    private void CheckMission()
    {

    }
}

[System.Serializable]
public class Mission
{
    public int missionProgress = 0;
    public int missionCap = 0;
}
