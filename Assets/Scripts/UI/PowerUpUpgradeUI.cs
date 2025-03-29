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
    public Slider progressSlider;
    bool isMaxed = false;
    Color defBtnColor;

    //TODO: Save current level;

    public float upgradeMultiplier = 1.5f;
    private void Awake()
    {
        image = GetComponentInChildren<Image>();
        button = GetComponentInChildren<Button>();
        text = button.GetComponentInChildren<TextMeshProUGUI>();
        progressSlider = GetComponentInChildren<Slider>();
        defBtnColor = button.colors.normalColor;
        progressSlider.interactable = false;
    }
    
    private void OnEnable()
    {
        CheckButtonColor();
        SetUI();
        CalculateProgressBar();
    }

    public void CalculateProgressBar()
    {
        progressSlider.maxValue = data.maxLevel;
        progressSlider.value = data.currentLevel-1;
    }
    public void CheckButtonColor()
    {
        if (isMaxed) return;
        if (data.defUpgradeCost>PlayerCollectibleManager.instance.GetGoldAmount())
        {
            ColorBlock cb = button.colors;
            cb.normalColor = Color.red;
            cb.selectedColor = Color.red;
            button.colors = cb;
        }
        else
        {
            ColorBlock cb = button.colors;
            cb.normalColor = defBtnColor;
            cb.selectedColor = defBtnColor;
            button.colors = cb;
        }
    }

    public void GetLevel()
    {
        if (isMaxed) return;

        
        
        if (PlayerCollectibleManager.instance.GetGoldAmount() < data.defUpgradeCost)
        {
            CheckButtonColor();
            return;
        }

        PlayerCollectibleManager.instance.AddCoin(-data.defUpgradeCost);
        data.currentLevel += 1;
        data.LevelUp();
        CalculateValue();
        SetUI();
        CheckButtonColor();

        CheckIfMaxed();

    }

    private void CheckIfMaxed()
    {
        if (data.currentLevel -1 == data.maxLevel)
        {
            text.text = "Maxed";
            ColorBlock cb = button.colors;
            cb.normalColor = Color.green;
            cb.selectedColor = Color.green;
            button.colors = cb;
            isMaxed = true;
            return;
        }
    }

    void SetUI()
    {
        text.text = data.defUpgradeCost.ToString();
        image.sprite = data.Image;
    }
    private void CalculateValue()
    {
        data.defUpgradeCost += data.defUpgradeCost/2 *  ((data.currentLevel - 1) * upgradeMultiplier);
        if (data.currentLevel > 4)
        {
            data.defUpgradeCost /= 2;
        }
    }
}
