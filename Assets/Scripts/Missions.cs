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


    public void AddToMission(Mission mission)
    {
        if(mission.isDone)
        {   
            print("Mission alreadyFinished ");
            return;
        }
        mission.AddToMission();

        if(mission.CheckMission())
        {
            IsMissionFinished();
        }

      
        
    }

    void IsMissionFinished(Mission mission){
        if(mission.isDone){
            GiveReward(mission);
        }


    }
    
    void GiveReward(Mission mission){
        // GameManager.instance.scoreMultiplier += mission.RewardPoints;
    }
  
}

[System.Serializable]
public class Mission
{
    public int missionProgress = 0;
    public int missionCap = 0;

    public bool isDone;
    public bool rewardX=1;

    public void CheckMission()
    {
        if(missionProgress == missionCap){
            isDone=true;
            return true;
        }
        else
            false;
    }

    public void AddToMission(){
        if(isDone) return;
        missionProgress++;
    }
}
