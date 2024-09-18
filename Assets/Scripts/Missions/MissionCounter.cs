using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionCounter : MonoBehaviour
{
    

    void TrainHitCounter()
    {
        Missions.Instance.AddToMission(Missions.Instance.trainHitMission);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("TrainSide") || collision.gameObject.CompareTag("TrainFront"))
        {
            TrainHitCounter();
        }
    }
}
