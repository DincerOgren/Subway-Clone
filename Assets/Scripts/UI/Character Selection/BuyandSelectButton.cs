using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuyandSelectButton : MonoBehaviour
{
    public SelectButtons defaultCharacter;
    SelectButtons selectedCharacter;
    private void Start()
    {
        selectedCharacter = defaultCharacter;
    }
    private void OnEnable()
    {
        Actions.onSelectCharacters += SelectedCharacter;
    }

    private void SelectedCharacter(SelectButtons v)
    {
        selectedCharacter = v;
    }

    public void Buy()
    {
        if (PlayerCollectibleManager.instance.GetGoldAmount() >= selectedCharacter.price)
        {
            PlayerCollectibleManager.instance.AddCoin(-selectedCharacter.price);
            selectedCharacter.isOwned = true;

            selectedCharacter.CheckButtons();
        }
        else
            print("Not enough currency");
    }

    public void Select()
    {

        if (Actions.onSelectButtonPressed != null)
        {
            Actions.onSelectButtonPressed.Invoke(selectedCharacter.modelRef.transform);

        }
        else
        {
            Debug.LogWarning("onSelectCharacters event is not assigned or has no listeners.");
        }
    }
}
