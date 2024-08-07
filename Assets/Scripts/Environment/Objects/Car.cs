using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Car : MonoBehaviour
{
    public float trainOffset = 8f;
    public float speed = 5f;
    // Start is called before the first frame update

    [Header("Gold Spawn")]
    public bool shouldSpawnGolds = true;
    [SerializeField] Transform goldSpawnLoc;
    [SerializeField] int goldSpawnAmount = 5;
    [SerializeField] GameObject coin;
    [SerializeField] Transform coinParent;
    [SerializeField] float goldSpaceBetween = 2f;
    [SerializeField] float coinZOffset = -3f;
    private void OnEnable()
    {

        speed = GameObject.FindWithTag("Player").transform.GetComponent<PlayerMovement>().GetSpeed();
        trainOffset = speed / 2f;
        if (shouldSpawnGolds)
        {
            StartCoroutine(SpawnCoin());

        }
        transform.position += Vector3.forward * trainOffset;

    }
    // Update is called once per frame
    void Update()
    {
        transform.Translate(0, 0, speed * Time.deltaTime);

    }

    IEnumerator SpawnCoin()
    {
        int i = 0;
        Vector3 temp = transform.position;
        while (i < goldSpawnAmount)
        {
            var coinObject = CoinPool(PoolingLists.coinList);
            print("PARENT POS: " + goldSpawnLoc.position);
            print("PARENT LOCALPOS: " + goldSpawnLoc.localPosition);
            coinObject.transform.position =
                new Vector3(temp.x, goldSpawnLoc.position.y, goldSpawnLoc.position.z + coinZOffset + i * goldSpaceBetween);
            print("POS: " + coinObject.transform.position.z);

            i++;
            coinObject.transform.parent = goldSpawnLoc;

            yield return null;
        }
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
}
