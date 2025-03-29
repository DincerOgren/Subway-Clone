using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionManager : MonoBehaviour, ISaveable
{
    public static MissionManager Instance;



    public MissionTier[] missionTiers;

    private void Awake()
    {
        missionTiers = new MissionTier[3];
        for (int i = 0; i < missionTiers.Length; i++)
        {
            missionTiers[i] = new MissionTier();
        }


        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        


    }
    private void Start()
    {
        //print(MissionManager.Instance.missionTiers[0].tierMissions[0].missionName);
        //print(MissionManager.Instance.missionTiers[0].tierMissions[1].missionName);
        //print(MissionManager.Instance.missionTiers[0].tierMissions[2].missionName);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {

            if (MissionManager.Instance == null)
            {
                Debug.LogError("NULLLLLL");
            }
            print(MissionManager.Instance.missionTiers[0].tierMissions[0].missionName);
            print(MissionManager.Instance.missionTiers[0].tierMissions[1].missionName);
            print(MissionManager.Instance.missionTiers[0].tierMissions[2].missionName);

        }
    }

    public Mission GetMissionByName(string missionName)
    {
        foreach (MissionTier tier in missionTiers)
        {
            foreach (Mission mission in tier.tierMissions)
            {
                if (mission.missionName == missionName)
                {
                    print("Mission found");
                    return mission;
                }
                else
                    continue;
                
            }
        }

        print("No match found for mission");

        return null;
    }
    public void AddToMission(Mission mission)
    {
        if (mission.isDone)
        {
            print("Mission alreadyFinished ");
            return;
        }
        mission.AddToMission();

        if (mission.CheckMission())
        {
            IsMissionFinished(mission);
        }



    }

    void IsMissionFinished(Mission mission)
    {
        if (mission.isDone)
        {
            GiveReward(mission);
        }


    }

    void GiveReward(Mission mission)
    {
        // GameManager.instance.scoreMultiplier += mission.RewardPoints;
        print("Reward Granted" + mission.rewardX);
    }

    public object CaptureState()
    {
        return missionTiers;
    }

    public void RestoreState(object v)
    {
        MissionTier[] tiers = (MissionTier[])v;

        missionTiers = tiers;
    }
}

[System.Serializable]

public class MissionTier
{
    public int missionTier = 0;
    public Mission[] tierMissions;

    public MissionTier()
    {
        tierMissions = new Mission[3]; // Initialize each tierMissions array
        for (int i = 0; i < tierMissions.Length; i++)
        {
            tierMissions[i] = new();
        }
    }
}

[System.Serializable]
public class Mission
{
    public string missionName;
    public int missionProgress = 0;
    public int missionCap = 0;
    public string missinInfo;

    public int missionSkipGold = 300;

    public bool isDone;
    public int rewardX = 1;


    // skipcost
    // mission info ui

    public bool CheckMission()
    {
        if (missionProgress == missionCap)
        {
            isDone = true;
            return true;
        }
        else
            return false;
    }

    public void AddToMission()
    {
        if (isDone) return;
        missionProgress++;
    }
}



