using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PowerUpUpgradeUI : MonoBehaviour
{
    public PowerUp data;
    public Image image;
    public TextMeshProUGUI text;
    public Button button;

    Color defBtnColor;

    //TODO: Save current level;

    public float upgradeMultiplier = 1.5f;
    private void Awake()
    {
        image = GetComponentInChildren<Image>();
        button = GetComponentInChildren<Button>();
        text = button.GetComponentInChildren<TextMeshProUGUI>();
        defBtnColor = button.colors.normalColor;
    }
    
    private void OnEnable()
    {

        CheckButtonColor();
        SetUI();
    }

    public void CheckButtonColor()
    {
        if (data.defUpgradeCost>PlayerCollectibleManager.instance.GetGoldAmount())
        {
            ColorBlock cb = button.colors;
            cb.normalColor = Color.red;
            button.colors = cb;
        }
        else
        {
            ColorBlock cb = button.colors;

            cb.normalColor = defBtnColor;
            button.colors = cb;
        }
    }

    public void GetLevel()
    {
        if (data.currentLevel==data.maxLevel)
        {
            text.text = "Maxed";
            return;
        }

        
        
        if (PlayerCollectibleManager.instance.GetGoldAmount() < data.defUpgradeCost)
        {
            CheckButtonColor();
            return;
        }

        CalculateValue();
        PlayerCollectibleManager.instance.AddCoin(-data.defUpgradeCost);
        data.currentLevel += 1;
        data.LevelUp();
        CalculateValue();
        SetUI();
        CheckButtonColor();
    }

    void SetUI()
    {
        text.text = data.defUpgradeCost.ToString();
        image.sprite = data.Image;
    }
    private void CalculateValue()
    {
        data.defUpgradeCost += data.defUpgradeCost/2 *  ((data.currentLevel - 1) * upgradeMultiplier);
    }
}
