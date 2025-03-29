using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionMaker : MonoBehaviour
{


    
    private void Start()
    {
        Tier1Missions();
    }

    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    if (MissionManager.Instance == null)
        //    {
        //        Debug.LogError("NULLLLLL");
        //    }
        //    else
        //        print(MissionManager.Instance.name);
            

        //}
    }

    public void Tier1Missions()
    {
        if (MissionManager.Instance != null)
        {
            print(MissionManager.Instance.missionTiers[0].tierMissions[0].missionName);


            MissionManager.Instance.missionTiers[0].tierMissions[0] = TrainHitMission();
            MissionManager.Instance.missionTiers[0].tierMissions[1] = RollMission();
            MissionManager.Instance.missionTiers[0].tierMissions[2] = JumpMission();

            //print(TrainHitMission().missionName + " Added ");
        }
        else
        {
            Debug.LogError("MissionManager.Instance is not initialized.");
        }

        //    MissionManager.Instance.missionTiers[0].tierMissions[1] = RollMission();
        //    MissionManager.Instance.missionTiers[0].tierMissions[2] = JumpMission();


    }

    

    Mission RollMission()
    {
        return CreateMission("RollMission", 0, 15, "Roll  15 times in one run", 300, 1);
    }
    Mission JumpMission()
    {
        return CreateMission("JumpMission", 0, 10, "Jump 10 times in one run", 500, 1);
    }
    Mission TrainHitMission()
    {
        return CreateMission("TrainHitMission", 0, 5, "Hit trains 5 times in one run", 300, 1);
    }
    public Mission CreateMission(string missionName, int progress, int cap, string info, int skipGold, int rewardX)
    {
        Mission newMission = new();
        newMission.missionName = missionName;
        newMission.missionProgress = progress;
        newMission.missionCap = cap;
        newMission.missinInfo = info;
        newMission.missionSkipGold = skipGold;
        newMission.rewardX = rewardX;

        print("Created " + newMission.missionName + " mission");

        return newMission;

    }
}


