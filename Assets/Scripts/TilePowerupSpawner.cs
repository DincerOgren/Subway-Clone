using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TilePowerupSpawner : MonoBehaviour
{
    [SerializeField] GameObject[] powerUpList;

    [SerializeField] float powerUpSpawnPercentage = 30;

    [SerializeField] float powerUpYThreshoýld = .3f;

    private void OnEnable()
    {
        SpawnPowerup(RollDicePercentage(powerUpSpawnPercentage));
    }

    bool RollDicePercentage(float value)
    {
        float temp = Random.Range(0, 100);

        if (temp <= value)
        {
            print("Power up spawned on " + transform.name);
            return true;

        }
        else
        {
            print("powerupo not spawned ");
            return false;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.O))
        {
            SpawnPowerup(RollDicePercentage(powerUpSpawnPercentage));

        }
    }

    void SpawnPowerup(bool v)
    {
        if (v)
        {
            var a = Instantiate(ChooseRandomPowerUp(), transform);
            a.transform.position = transform.position + Vector3.up * powerUpYThreshoýld;
        }

    }

    private GameObject ChooseRandomPowerUp()
    {

        return powerUpList[Random.Range(0, powerUpList.Length)];


    }
}
