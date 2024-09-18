using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarTrigger : MonoBehaviour
{
    [SerializeField] GameObject train;
    [SerializeField] Transform spawnLoc;


    [SerializeField] float spawnTimeDelay;
    [SerializeField] float timeBetweenSpawns;
    [SerializeField] float distanceFromPlayer = 15;
    [SerializeField] int spawnAmount = 5;


    Transform playerRef;
    private void Awake()
    {
        playerRef = GameObject.FindWithTag("Player").transform;
    }
    private void Start()
    {
        float speed= playerRef.GetComponent<PlayerMovement>().GetSpeed();
        spawnTimeDelay = 15 / speed;
        timeBetweenSpawns = 1 - (speed * 0.01f);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            var a = Instantiate(train, spawnLoc.position, Quaternion.Euler(0, 180, 0));
            a.transform.parent = GameManager.Instance.spawnedTrainParent;
            StartCoroutine(SpawnTrains());

        }
    }

    IEnumerator SpawnTrains()
    {
        float elapsedTime =0;
        
        while (elapsedTime < spawnTimeDelay)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        int i = 1;
        float timer = 0;
        while (true)
        {
            if (i == spawnAmount)
            {
                yield break;
            }
            if (timer > timeBetweenSpawns)
            {
                timer = 0;
               

                if (i % 2 != 0)
                {

                   var a = Instantiate(train, new Vector3(0, 0, playerRef.position.z + distanceFromPlayer), Quaternion.Euler(0, 180, 0));
                    a.transform.parent = GameManager.Instance.spawnedTrainParent;

                }
                else if (i % 2 == 0)
                {
                    var a = Instantiate(train, new Vector3(3, 0, playerRef.position.z + distanceFromPlayer), Quaternion.Euler(0, 180, 0));
                    var b =Instantiate(train, new Vector3(-3, 0, playerRef.position.z + distanceFromPlayer), Quaternion.Euler(0, 180, 0));
                    a.transform.parent = GameManager.Instance.spawnedTileParent;
                    b.transform.parent = GameManager.Instance.spawnedTileParent;

                }
                i++;
            }



            timer += Time.deltaTime;

            yield return null;
        }


    }
}
