using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    Transform _playerRef;
    Vector3 _startPosition;
    public Transform spawnedTileParent;
    public Transform spawnedTrainParent;
    public Transform objectParent;
    public Transform planeParent;

    private void Awake()
    {
        _playerRef = GameObject.FindWithTag("Player").transform;
        _startPosition = _playerRef.position; 
    }
    private void Start()
    {
        if (Instance == null)
        {

            Instance = this;
        }
        else
            Destroy(gameObject);
    }


    public void ResetPlayerLocation()
    {
        _playerRef.position = _startPosition;
    }

    public void DeleteAllChilds()
    {
        for (int i = 0; i < spawnedTileParent.childCount; i++)
        {
            Destroy(spawnedTileParent.GetChild(i).gameObject);
        }

        for (int i = 0; i < spawnedTrainParent.childCount; i++)
        {
            Destroy(spawnedTrainParent.GetChild(i).gameObject);
        } 
        
        for (int i = 0; i < objectParent.childCount; i++)
        {
            Destroy(objectParent.GetChild(i).gameObject);
        } 

        for (int i = 0; i < planeParent.childCount; i++)
        {
            Destroy(planeParent.GetChild(i).gameObject);
        }
    }
    public void DeleteChilds(Transform value)
    {
        for (int i = 0; i < value.childCount; i++)
        {
            Destroy(value.GetChild(i).gameObject);
        }
    }
}