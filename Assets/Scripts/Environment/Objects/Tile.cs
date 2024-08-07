using System.Collections;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public GameObject train;
    public GameObject playerRef;
    public int spawnAmount = 4;
    public float timeBetweenSpawns = 2f;
    public Transform[] spawnLocations;

    public float spawnTimeDelay = 2f;

    int triggerEnterTimes = 0;
    private void OnEnable()
    {
        playerRef = GameObject.FindWithTag("Player");
        //spawnTimeDelay = 15 / playerRef.GetComponent<PlayerMovement>().GetSpeed();
    }



    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            print("Trigger = "+triggerEnterTimes);
            SpawnTrain();
            //StartCoroutine(SpawnTrains());

        }
    }

    void SpawnTrain()
    {
        int random = Random.Range(0, 2);

        if (random == 1)
        {
            Instantiate(train, spawnLocations[triggerEnterTimes].position + Vector3.right * 3, Quaternion.Euler(0, 180, 0));
        }
        else
            Instantiate(train, spawnLocations[triggerEnterTimes].position + Vector3.right * -3, Quaternion.Euler(0, 180, 0));

        triggerEnterTimes++;

    }
    //IEnumerator SpawnTrains()
    //{
    //    float elapsedTime = 0;

    //    while (elapsedTime < spawnTimeDelay)
    //    {
    //        elapsedTime += Time.deltaTime;
    //        yield return null;
    //    }
    //    int i = 0;
    //    float timer = 0;

    //    while (true)
    //    {
    //        print("While içi");
    //        if (i == spawnAmount)
    //        {
    //            break;
    //        }
    //        if (timer >= timeBetweenSpawns)
    //        {
    //            timer = 0;

    //            int random = Random.Range(0, 2);

    //            print("Random=  " + random);
    //            if (random == 1)
    //            {
    //                var trainObject = Instantiate(train, spawnLocationR.position, Quaternion.Euler(0, 180, 0));
    //                print("Spawned on right");
    //            }
    //            else
    //            {
    //                var trainObject = Instantiate(train, spawnLocationL.position + Vector3.forward * playerRef.GetComponent<PlayerMovement>().GetSpeed(), Quaternion.Euler(0, 180, 0)) ;
    //                print("Spawned on left");

    //            }
    //            i++;
    //        }

    //        timer += Time.deltaTime;
    //        yield return null;
    //    }

    //}

}
