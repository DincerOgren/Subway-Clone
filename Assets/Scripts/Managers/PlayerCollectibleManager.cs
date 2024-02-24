using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollectibleManager : MonoBehaviour
{
    public static PlayerCollectibleManager instance;


    [SerializeField] float coinAmount = 0;
    void Start()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

 

    public void AddCoin(float v = 1) => coinAmount += v;

    public float GetGoldAmount() => coinAmount;
    public void ResetCoins()
    {
        coinAmount = 0;
    }

}
