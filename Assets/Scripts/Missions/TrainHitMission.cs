using UnityEngine;

public class TrainHitMission : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("TrainSide") || collision.gameObject.CompareTag("TrainFront"))
        {
            MissionCounter.Instance.TrainHitCounter();
        }
    }
}
