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
            IsMissionFinished(mission);
        }

      
        
    }

    void IsMissionFinished(Mission mission){
        if(mission.isDone){
            GiveReward(mission);
        }


    }
    
    void GiveReward(Mission mission){
        // GameManager.instance.scoreMultiplier += mission.RewardPoints;
        print("Reward Granted" + mission.rewardX);
    }
  
}

//[System.Serializable]
//public class Mission
//{
//    public int missionProgress = 0;
//    public int missionCap = 0;

//    public bool isDone;
//    public int rewardX = 1;

//    // skipcost
//    // mission info ui
    
//    public bool CheckMission()
//    {
//        if(missionProgress == missionCap){
//            isDone=true;
//            return true;
//        }
//        else
//            return false;
//    }

//    public void AddToMission(){
//        if(isDone) return;
//        missionProgress++;
//    }
//  }

