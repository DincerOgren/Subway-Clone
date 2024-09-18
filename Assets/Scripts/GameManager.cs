using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    Vector3 _startPosition;
    public Transform spawnedTileParent;
    public Transform spawnedTrainParent;
    public Transform objectParent;
    public Transform planeParent;


    [SerializeField] GameObject _planePrefab;

    ChasingEnemy _enemyRef;
    Transform _playerRef;
    private void Awake()
    {
        _playerRef = GameObject.FindWithTag("Player").transform;
        _enemyRef = GameObject.FindWithTag("Enemy").GetComponent<ChasingEnemy>();
        _startPosition = _playerRef.position; 
    }
    private void Start()
    {

        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    void SpawnStartTile()
    {
        var a= Instantiate(_planePrefab, planeParent);
        a.transform.position = Vector3.zero;
    }
    public void ResetPlayerLocation()
    {
        _playerRef.gameObject.SetActive(true);
        _playerRef.position = _startPosition;
        _playerRef.GetComponent<Health>().ResetDeadState();
    }

    public void RestartSequence()
    {
        DeleteAllChilds();
        SpawnStartTile();

        ResetPlayerLocation();

        ResetEnemy();

        GameStarter.Instance.RestartGame();

        ChunkSpawner.Instance.SpawnChunk();


    }

    void ResetEnemy()
    {
        _enemyRef.ResetChasingEnemy();
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
