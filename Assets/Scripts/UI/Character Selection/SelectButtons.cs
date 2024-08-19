using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SelectButtons : MonoBehaviour
{
    public bool isOwned;

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
        CheckButtons();
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
            CheckButtons();
            selectedMark.SetActive(true);
        }
        else
        {
            Debug.LogWarning("onSelectCharacters event is not assigned or has no listeners.");
        }
    }
}
