using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CoinUI : MonoBehaviour
{
    public RectTransform backgroundPanel;
    public TextMeshProUGUI coinText;
    public float coins = 0;
    public float padding = 50f;

    void Start()
    {
        coins = -1;
        UpdateCoin();
        coinText.text = "0";
    }


    private void OnEnable()
    {
        Actions.OnCoinTakeInGame += UpdateCoin;
    }
    private void UpdateCoin()
    {
        coins += 1;
        coinText.text = coins.ToString();

        float preferredWidth = coinText.preferredWidth;

        backgroundPanel.sizeDelta = new Vector2(preferredWidth + padding, backgroundPanel.sizeDelta.y);
    }
}
