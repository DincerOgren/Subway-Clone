using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionCounter : MonoBehaviour
{
    public static MissionCounter Instance;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void TrainHitCounter()
    {
        Mission msn = MissionManager.Instance.GetMissionByName("TrainHitMission");
        if(msn == null)
        {
            print("Coulnt find jump misson in list");
        }
        else
            MissionManager.Instance.AddToMission(msn);
    }
    
    public void JumpCounter()
    {
        MissionManager.Instance.AddToMission(MissionManager.Instance.GetMissionByName("JumpMission"));
    }
    public void RollCounter()
    {
        MissionManager.Instance.AddToMission(MissionManager.Instance.GetMissionByName("RollMission"));
        print("ROLL ROLL ROLL");
    }
    //private void OnCollisionEnter(Collision collision)
    //{
    //    if (collision.gameObject.CompareTag("TrainSide") || collision.gameObject.CompareTag("TrainFront"))
    //    {
    //        TrainHitCounter();
    //    }
    //}
}
