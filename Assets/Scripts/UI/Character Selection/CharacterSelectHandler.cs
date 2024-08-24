using System;
using System.Collections;
using System.Collections.Generic;
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
        Actions.onSelectCharacters += SelectCharacter;
        Actions.onSelectButtonPressed += SelectPlayerModel;
        Actions.onCharacterHovered += SelectDisplayModel;
    }

    public void SelectCharacter(SelectButtons v)
    {
        for (int i = 0; i < characters.Length; i++)
        {
            characters[i].selectedMark.SetActive(false);
        }

        v.selectedMark.SetActive(true);
    }

    void SelectPlayerModel(Transform selectedModel)
    {
        foreach (var item in _playerModels)
        {
            item.SetActive(false);

            if (selectedModel.name == item.name)
            {
                item.SetActive(true);
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

    
}
