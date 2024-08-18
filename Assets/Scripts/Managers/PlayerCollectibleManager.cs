using UnityEngine;

public class PlayerCollectibleManager : MonoBehaviour
{
    public static PlayerCollectibleManager instance;

    [Header("Gold")]
    [SerializeField] float currentCoinAmount = 0;
    [SerializeField] float coinAmount = 0;

    [Header("Skate")]
    [SerializeField] float skateAmount = 0;

   

    void Start()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);


        Actions.onSkateChange?.Invoke(skateAmount);

        Actions.onCoinChange?.Invoke(coinAmount);


        DontDestroyOnLoad(gameObject);
    }




 
    public bool CheckSkate()
    {
        return skateAmount > 0;
    }
    public void UseSkate()
    {
        skateAmount -= 1;
    }
    public void AddSkate(float v = 1) 
    {
        skateAmount += v;
        Actions.onSkateChange?.Invoke(skateAmount);

    }



    public void AddCoin(float v = 1)
    {
        coinAmount += v;
        Actions.onCoinChange?.Invoke(coinAmount);
    }
    public void AddToCurrentCoin(float v = 1)
    {
        currentCoinAmount += v;
    }
    public void AddCurrentCoinsToCoins()
    {
        coinAmount += currentCoinAmount;
    }
    public float GetGoldAmount() => coinAmount;
    public float GetCurrentCoins() => currentCoinAmount;
    public void ResetCoins()
    {
        currentCoinAmount = 0;
    }

}
