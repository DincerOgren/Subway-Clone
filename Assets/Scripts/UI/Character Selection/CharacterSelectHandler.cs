using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class CharacterSelectHandler : MonoBehaviour
{
    SelectButtons[] characters;
    public GameObject displayPlayer;
    public GameObject playerModelRef;


    GameObject[] _playerModels;
    GameObject[] _playerDisplayModels;
    private void Awake()
    {
        playerModelRef = GameObject.FindWithTag("PlayerModel");
        _playerModels = new GameObject[playerModelRef.transform.childCount];
        _playerDisplayModels = new GameObject[displayPlayer.transform.childCount];

        characters = new SelectButtons[transform.childCount];

        for (int i = 0; i < transform.childCount; i++)
        {
            characters[i] = transform.GetChild(i).GetComponent<SelectButtons>();
        }

        for (int i = 0; i < playerModelRef.transform.childCount; i++)
        {
            _playerModels[i] = playerModelRef.transform.GetChild(i).gameObject;
        }
        for (int i = 0; i < displayPlayer.transform.childCount; i++)
        {
            _playerDisplayModels[i] = displayPlayer.transform.GetChild(i).gameObject;
        }
    }

    private void OnEnable()
    {
        CheckSelected();
        Actions.onSelectCharacters += SelectCharacter;
        Actions.onSelectButtonPressed += SelectPlayerModel;
        Actions.onCharacterHovered += SelectDisplayModel;
    }

    private void OnDisable()
    {
        Actions.onSelectCharacters -= SelectCharacter;
        Actions.onSelectButtonPressed -= SelectPlayerModel;
        Actions.onCharacterHovered -= SelectDisplayModel;
    }
    void CheckSelected()
    {
        SelectButtons activeCharacter = FindSelected();

        SelectCharacter(activeCharacter);
        SelectDisplayModel(activeCharacter.modelRef.transform);
    }
    SelectButtons FindSelected()
    {
        foreach (var item in characters)
        {
            if (item.isSelected == true)
            {
                return item;
            }
        }


        return null;
    }
    public void SelectCharacter(SelectButtons v)
    {
        for (int i = 0; i < characters.Length; i++)
        {
            characters[i].selectedMark.SetActive(false);

        }
        
        v.selectedMark.SetActive(true);
        v.CheckButtons();

    }

    void SelectPlayerModel(SelectButtons v)
    {
        Transform selectedModel = v.modelRef.transform;

        foreach (var item in _playerModels)
        {
            item.SetActive(false);
            if (selectedModel.name == item.name)
            {
                item.SetActive(true);
                SetChoosenCharacter(v);
            }
        }
        
        
    }
    
    void SelectDisplayModel(Transform selectedModel)
    {
       
        foreach (var item in _playerDisplayModels)
        {
            item.SetActive(false);

            if (selectedModel.name == item.name)
            {
                item.SetActive(true);
            }
        }

    }

    void SetChoosenCharacter(SelectButtons v)
    {
        for (int i = 0; i < characters.Length; i++)
        {
            characters[i].isSelected = false;
        }

        v.isSelected = true;
    }

    
}
