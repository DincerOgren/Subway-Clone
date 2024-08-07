using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Timeline;

public class CoinSpawner : MonoBehaviour
{
    public int goldSpawnAmount;
    public float goldSpaceBetween;
    public float coinZOffset;
    private Transform goldSpawnLoc;
    public float maxWaveHeight = 5f;
    public Transform coinParent;
    public GameObject coin;

    public float playerSpeedDivider = 2;

    PlayerMovement player;
    // Start is called before the first frame update
    private void OnEnable()
    {
        goldSpawnLoc = transform;
        player = GameObject.FindWithTag("Player").GetComponent<PlayerMovement>();
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(SpawnCoin());

        }
    }
    IEnumerator SpawnCoin()
    {
        int i = 0;
        Vector3 temp = transform.position;
        // Set your desired max height here
        float waveLength = goldSpawnAmount-1; // Adjust the wavelength as needed
        float frequency = Mathf.PI  / waveLength; // Calculate the frequency for one complete wave cycle
        float playerSpeedOffset = player.GetSpeed()/playerSpeedDivider;

        while (i < goldSpawnAmount)
        {
            var coinObject = CoinPool(PoolingLists.coinList);
            float offset = goldSpaceBetween;

            // Calculate the new position using a sine wave
            float heightOffset = Mathf.Sin(i * frequency) * maxWaveHeight;
            coinObject.transform.position = new Vector3(temp.x, goldSpawnLoc.position.y + heightOffset, goldSpawnLoc.position.z + i * offset);

            coinObject.transform.parent = coinParent;

            i++;
            yield return null; // Adjust the delay to control the spawn rate
        }
    }
    //IEnumerator SpawnCoin()
    //{
    //    int i = 0;
    //    Vector3 temp = transform.position;
    //    bool goingUp = true; // flag to control direction
    //    int center = Mathf.FloorToInt(goldSpawnAmount / 2);
    //    print("Centre = " + center);
    //    while (i < goldSpawnAmount)
    //    {
    //        var coinObject = CoinPool(PoolingLists.coinList);
    //        float offset = goldSpaceBetween;

    //        // Toggle direction and calculate the new position
    //        if (goingUp)
    //        {

    //            coinObject.transform.position = new Vector3(temp.x, goldSpawnLoc.position.y + i*offset*2, goldSpawnLoc.position.z-(center-i)*coinZOffset + i*(offset/2));
    //        }
    //        else
    //        {
    //            print("i " +  (goldSpawnAmount - i));
    //            coinObject.transform.position = new Vector3(temp.x, goldSpawnLoc.position.y + (goldSpawnAmount- i-1)* offset * 2, goldSpawnLoc.position.z - (center - i) * coinZOffset + i* (offset / 2));
    //        }

    //        coinObject.transform.parent = coinParent;

    //        // Switch direction after a complete wave
    //        if (i == center)
    //        {
    //            goingUp = false;
    //        }

    //        i++;
    //        yield return null; // adjust the delay to control the spawn rate
    //    }
    //}

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

}
