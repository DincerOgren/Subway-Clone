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

    public delegate void OnSelectCharacters(SelectButtons v);
    public static OnSelectCharacters onSelectCharacters;

    public delegate void OnCharacterSelectButtonPress(Transform v);
    public static OnCharacterSelectButtonPress onSelectButtonPressed;
    public static OnCharacterSelectButtonPress onCharacterHovered;

}