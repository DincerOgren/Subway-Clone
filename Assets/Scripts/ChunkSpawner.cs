using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class ChunkSpawner : MonoBehaviour
{
    public static ChunkSpawner Instance;
    [SerializeField] GameObject[] tiles;

    List<GameObject> spawnedChunks = new List<GameObject>();
    public float spawnOffset = 100;
    const float firstChunkOffset = 44.5f;




    private void Start()
    {
        StartCoroutine(Wait());
        Instance = this;

        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }
        Instance = this;
        //DontDestroyOnLoad(this.gameObject);
        //adddelay
        //SpawnChunk();
    }
    
    IEnumerator Wait()
    {
        yield return null;
        SpawnChunk(true);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("ChunkEnd"))
        {
            print("pos=" + other.transform.position);
            print("localPos = " + other.transform.localPosition);
            SpawnChunk(false,other);

            //Transform parentTransform = other.transform.parent;

            //if (parentTransform != null)
            //{
            //    parentTransform.gameObject.SetActive(false);
            //    Debug.Log("Parent objesi deaktif edildi: " + parentTransform.name);
            //}
            //else
            //{
            //    Debug.Log("Bu objenin parent'ý yok: " + other.name);
            //}
        }
    }

    public void SpawnChunk(bool isFirstChunk=false,Collider other=null)
    {
        
        var selectedChunk = ChooseRandomChunk();

        if (CheckChunkExsistedInList(selectedChunk))
        {
            ActivateAllObjectsInChunk(selectedChunk);
        }
        else
        {
            spawnedChunks.Add(selectedChunk);
            ActivateAllObjectsInChunk(selectedChunk);
        }

        if (isFirstChunk)
        {
            var a = Instantiate(selectedChunk, new Vector3(0, 0, transform.position.z + firstChunkOffset), Quaternion.identity);
            print(a.name);
            a.transform.parent = GameManager.Instance.spawnedTileParent;
        }
        else if (other != null) 
        {
            var a = Instantiate(selectedChunk, new Vector3(0, 0, other.transform.position.z + spawnOffset), Quaternion.identity);
            print(a.name);
            a.transform.parent = GameManager.Instance.spawnedTileParent;

        }


    }

    private void ActivateAllObjectsInChunk(GameObject selectedChunk)
    {

            selectedChunk.SetActive(true);

            // Recursively enable all children
            foreach (Transform child in selectedChunk.transform)
            {
                ActivateAllObjectsInChunk(child.gameObject);
            }
        

    }

    private bool CheckChunkExsistedInList(GameObject chunk)
    {
      return  spawnedChunks.Contains(chunk);
    }

    private GameObject ChooseRandomChunk()
    {
        if (tiles.Length<1)
        {
            print("ERROR");
            return null;
        }


        int randomNum = Random.Range(0, tiles.Length);

        return tiles[randomNum];    
    }

    
}
