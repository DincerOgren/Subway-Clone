using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainSpawner : MonoBehaviour
{

    [SerializeField] private GameObject _trainPrefab;
    [SerializeField] private float spawnTimeDelay;
    [SerializeField] private Transform[] spawnLocations;




    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(SpawnTrains());
        }
    }


    IEnumerator SpawnTrains()
    {
        int i = 0;
        while (true)
        {
            if (i>=spawnLocations.Length)
            {
                break;
            }

            Instantiate(_trainPrefab, spawnLocations[i].position, Quaternion.Euler(0, 180, 0));


            i++;

            yield return null;
        }
    }
}
