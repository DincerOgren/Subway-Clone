using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MissionBlock : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI missinInfoText;

    [SerializeField]
    Slider missionProgressSlider;
    [SerializeField]
    TextMeshProUGUI missionSkipGoldAmount;
    [SerializeField]
    TextMeshProUGUI sliderText;
    [SerializeField]
    Button skipMissionButton;
    [SerializeField]
    float missionSkipGold = 100;


    bool canBuy = false;

    private void OnEnable()
    {
        CheckIfPlayerCanBuyIt();
    }

    public void UpdateMissinInfo(Mission mission)
    {
        missinInfoText.text = mission.missinInfo;
        missionProgressSlider.maxValue = mission.missionCap;
        missionProgressSlider.value = mission.missionProgress;
        missionSkipGoldAmount.text = mission.missionSkipGold.ToString();
        sliderText.text = $"{missionProgressSlider.value}/{missionProgressSlider.maxValue}";
    }

    public void CheckIfPlayerCanBuyIt()
    {
        if (PlayerCollectibleManager.instance.GetGoldAmount() >= missionSkipGold)
        {
            skipMissionButton.interactable = true;
            canBuy = true;
        }
        else
        {
            skipMissionButton.interactable = false;
            canBuy = false;
        }

    }
    public void SkipButton()
    {
        if (!canBuy) return;

        PlayerCollectibleManager.instance.AddCoin(-missionSkipGold);
        print("Bought button");
        gameObject.SetActive(false);
    }
}
