using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionsUI : MonoBehaviour
{
    [SerializeField]
    MissionBlock UIPrefab;

    public int tierNum=0;

    public int perMissionByTier = 3;
    private void OnEnable()
    {
        DestroyAllChilds();
        SpawnTierMissions(tierNum);
    }

    private void SpawnTierMissions(int tier)
    {
        for (int i = 0; i < perMissionByTier; i++)
        {
            var a = Instantiate(UIPrefab, transform);

            a.UpdateMissinInfo(MissionManager.Instance.missionTiers[tier].tierMissions[i]);
        }
    }

    void DestroyAllChilds()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            Destroy(transform.GetChild(i).gameObject);
        }
    }

    void Update()
    {
        
    }
}
