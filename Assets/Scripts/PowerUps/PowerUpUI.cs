using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PowerUpUI : MonoBehaviour
{
    public PowerUp powerUpData;
    Slider slider;
    public Image powerUpImage;
    public TextMeshProUGUI amountText;

    private void Awake()
    {
    
        powerUpImage = GetComponentInChildren<Image>();     

        slider = GetComponentInChildren<Slider>();
    }
    private void Start()
    {
        slider.maxValue = powerUpData.Duration;
        slider.value = powerUpData.Duration;

        powerUpImage.sprite = powerUpData.Image;
    }


    private void Update()
    {
        CheckSlider();
    }

    private void CheckSlider()
    {
        if (slider.value == 0) 
        {
            gameObject.SetActive(false);        
        }
        slider.value = powerUpData.Duration - powerUpData.Timer;
    }
}
