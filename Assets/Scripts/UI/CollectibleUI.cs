using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CollectibleUI : MonoBehaviour
{
    TextMeshProUGUI text;
    public bool isGoldCollectible = false;

    private void Awake()
    {

        text = GetComponentInChildren<TextMeshProUGUI>();

    }

    private void OnEnable()
    {
        if (isGoldCollectible)
        {
            Actions.onCoinChange += SetUI;
        }
        else
            Actions.onSkateChange += SetUI;
    }
    private void Start()
    {
    }

    public void SetUI(float v)
    {
        text.text = v.ToString();
    }
}
