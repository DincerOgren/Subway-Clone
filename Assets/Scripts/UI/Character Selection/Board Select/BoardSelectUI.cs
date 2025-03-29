using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class BoardSelectUI : MonoBehaviour
{
    public bool isOwned;
    public bool isSelected;

    public float price = 500f;

    [Header("Skate Model Ref")]
    public GameObject modelRef;
    [SerializeField] GameObject displayBoard;

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

            if (isSelected)
            {

                selectButton.GetComponentInChildren<TextMeshProUGUI>().text = "Selected";
                selectButton.interactable = false;
                print("Selected " + transform.name);
            }
            else
            {
                selectButton.interactable = true;

                selectButton.GetComponentInChildren<TextMeshProUGUI>().text = "Select";
                print("Select " + transform.name);


            }
            
            buyButton.gameObject.SetActive(false);
            selectButton.gameObject.SetActive(true);
            checkMark.SetActive(true);
        }
    }

    public void Select()
    {
        if (Actions.onSelectBoards != null)
        {
            Actions.onSelectBoards.Invoke(this);
            CheckButtons();
            selectedMark.SetActive(true);
            Actions.onBoardHovered.Invoke(modelRef.transform);
        }
        else
        {
            Debug.LogWarning("onSelectCharacters event is not assigned or has no listeners.");
        }
    }
}
