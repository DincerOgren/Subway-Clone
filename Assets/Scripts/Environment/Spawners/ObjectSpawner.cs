using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{
    [SerializeField] GameObject[] obstacles;
    [SerializeField] GameObject[] objectsWithoutOffset;
    [SerializeField] GameObject coin;
    [SerializeField] Transform obstacleParent;
    [SerializeField] Transform coinParent;

    [SerializeField] float spawnDistance = 10f;
    [SerializeField] int goldSpawnAmount = 5;
    [SerializeField] float goldSpaceBetween= 2f;

    [SerializeField] LayerMask obstacleLayer;


    float playerSpeed;
    Transform playerRef;
    private void Awake()
    {
        playerRef = GameObject.FindWithTag("Player").transform;
    }
    private void Update()
    {
        Vector3 temp = playerRef.position;
        transform.position = new Vector3(transform.position.x, transform.position.y, temp.z);
        playerSpeed = playerRef.GetComponent<PlayerMovement>().GetSpeed();
       
    }


    private void SpawnObject()
    {

                
        int randomValue = Random.Range(0, 3);
        // %66 chance to spawn object
        if (randomValue == 1 || randomValue == 2)
        {
            //1 3
            int temp = Random.Range(1, 2);
            if (temp == 1)
            {

                var chosenObject = objectsWithoutOffset[Random.Range(0, objectsWithoutOffset.Length)];

                
                var obje = InstantiateOrReuse(PoolingLists.objectsWithoutOffsetList, chosenObject, obstacleParent);

                obje.transform.position = new Vector3(transform.position.x, transform.position.y - 0.6f, transform.position.z + (playerSpeed / 2 + spawnDistance));

                if (obje.transform.childCount >= 1)
                {
                    foreach (Transform child in obje.transform)
                    {
                        child.gameObject.SetActive(true);
                    }
                }
                obje.SetActive(true);

             

            }
            if (temp == 2)
            {
                var chosenObject = obstacles[Random.Range(0, obstacles.Length)];

                var obje = InstantiateOrReuse(PoolingLists.obstacleList, chosenObject, obstacleParent);
                obje.transform.position = transform.position + Vector3.forward * (playerSpeed / 2 + spawnDistance);

                if (obje.transform.childCount >= 1)
                {
                    foreach (Transform child in obje.transform)
                    {
                        child.gameObject.SetActive(true);
                    }
                }
                obje.SetActive(true);
            }

        }
        else if (randomValue == 0)
        {
            StartCoroutine(SpawnCoin());
        }
    }

    GameObject InstantiateOrReuse(List<GameObject> list,GameObject spawnedObject,Transform parent)
    {
        for (int i = 0; i < list.Count; i++)
        {
           
            if (list[i] != null && !list[i].activeSelf && list[i].name == (spawnedObject.name+"(Clone)"))
            {
            
                
                return list[i];
            }
        }

        list.Add(Instantiate(spawnedObject,parent));

        return list[^1];
    }

    GameObject CoinPool(List<GameObject> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i] != null && !list[i].activeSelf)
            {
                list[i].SetActive(true);
                return list[i];
            }

        }

        list.Add(Instantiate(coin, coinParent));

        return list[^1];
    }
    IEnumerator SpawnCoin()
    {
        int i = 0;
        Vector3 temp = transform.position;
        while (i<goldSpawnAmount)
        {
            var coinObject = CoinPool(PoolingLists.coinList);
            coinObject.transform.position = 
                new Vector3(temp.x, temp.y, temp.z + playerSpeed / 2 + (spawnDistance - 4) + i * goldSpaceBetween);
            
            i++;
            yield return null;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("ObjectSpawner"))
        {
            
            SpawnObject();
        }
    }
    

}
