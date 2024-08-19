using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSelectHandler : MonoBehaviour
{
    SelectButtons[] characters;

    private void Awake()
    {
        characters = new SelectButtons[transform.childCount];

        for (int i = 0; i < transform.childCount; i++)
        {
            characters[i] = transform.GetChild(i).GetComponent<SelectButtons>();
        }
    }

    private void OnEnable()
    {
        Actions.onSelectCharacters += SelectCharacter;
    }

    public void SelectCharacter(SelectButtons v)
    {
        for (int i = 0; i < characters.Length; i++)
        {
            characters[i].selectedMark.SetActive(false);
        }

        v.selectedMark.SetActive(true);
    }

    
}
