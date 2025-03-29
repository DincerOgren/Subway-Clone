using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SelectButtons : MonoBehaviour , ISaveable
{
    public bool isOwned;

    public bool isSelected;

    public float price = 500f;

    [Header("Model Ref")]
    public GameObject modelRef;
    public GameObject characterDisplay;

    [Header("Selected Items")]
    public GameObject checkMark;
    public GameObject selectedMark;

    public Button selectButton;
    public Button buyButton;

    private void OnEnable()
    {
    }

    public void CheckButtons()
    {
        if (!isOwned)
        {
            buyButton.GetComponentInChildren<TextMeshProUGUI>().text = price.ToString();
            buyButton.gameObject.SetActive(true);
            selectButton.gameObject.SetActive(false);
        }
        else
        {
            //isOwned
            
            if (isSelected)
            {
                
                selectButton.GetComponentInChildren<TextMeshProUGUI>().text = "Selected";
                selectButton.interactable = false;
            }
            else
            {
                selectButton.interactable = true;

                selectButton.GetComponentInChildren<TextMeshProUGUI>().text = "Select";

            }
            buyButton.gameObject.SetActive(false);
            selectButton.gameObject.SetActive(true);
            checkMark.SetActive(true);

        }
    }


    public void Select()
    {
        if (Actions.onSelectCharacters != null)
        {
            Actions.onSelectCharacters.Invoke(this);
            selectedMark.SetActive(true);
            Actions.onCharacterHovered.Invoke(modelRef.transform);
            CheckButtons();
        }
        else
        {
            Debug.LogWarning("onSelectCharacters event is not assigned or has no listeners.");
        }
    }

    public object CaptureState()
    {
        return isOwned;
    }

    public void RestoreState(object v)
    {
        bool ownStatus = (bool)v;
        isOwned = ownStatus;
    }
}
