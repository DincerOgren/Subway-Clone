using System;
using UnityEngine;

public static class Actions
{

    public delegate GameObject OnGeneratePlane(float v);
    public static OnGeneratePlane GeneratePlane;

    public delegate void OnCoinChange(float v);
    public static OnCoinChange onCoinChange;

    public delegate void OnSkateAdded(float v);
    public static OnSkateAdded onSkateChange;

    // Player Model Select UI
    public delegate void OnSelectCharacters(SelectButtons v);
    public static OnSelectCharacters onSelectCharacters;

    public delegate void OnCharacterSelectButtonPress(Transform v);
    public static OnCharacterSelectButtonPress onCharacterHovered;

    public delegate void OnCharacterSelectButtonPressed(SelectButtons s);
    public static OnCharacterSelectButtonPressed onSelectButtonPressed;

    // Board Model Select UI
    public delegate void OnSelectBoards(BoardSelectUI v);
    public static OnSelectBoards onSelectBoards;

    public delegate void OnBoardSelectButtonPress(Transform v);
    public static OnBoardSelectButtonPress onBoardHovered;
    

    public delegate void OnBoardSelectPressed(BoardSelectUI s);
    public static OnBoardSelectPressed onBoardSelectPressed;


    public static Action OnCoinTakeInGame;
    

}