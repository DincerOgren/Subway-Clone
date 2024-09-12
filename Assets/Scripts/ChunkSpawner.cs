using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class ChunkSpawner : MonoBehaviour
{
    [SerializeField] GameObject[] tiles;

    List<GameObject> spawnedChunks = new List<GameObject>();
    public float spawnOffset = 20;



    private void Start()
    {
        SpawnChunk();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("ChunkEnd"))
        {
            SpawnChunk();

            Transform parentTransform = other.transform.parent;

            if (parentTransform != null)
            {
                parentTransform.gameObject.SetActive(false);
                Debug.Log("Parent objesi deaktif edildi: " + parentTransform.name);
            }
            else
            {
                Debug.Log("Bu objenin parent'ý yok: " + other.name);
            }
        }
    }

    private void SpawnChunk()
    {
        print("SpawnChunk");
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

        var a = Instantiate(selectedChunk, new Vector3(0,0,transform.position.z + spawnOffset), Quaternion.identity);
        a.transform.parent = GameManager.Instance.spawnedTileParent;

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
