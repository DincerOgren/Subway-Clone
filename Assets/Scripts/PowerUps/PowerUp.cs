using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PowerUp", menuName = "ScriptableObjects/PowerUp", order = 2)]
public class PowerUp : ScriptableObject
{
    public PowerUpType powerUpType;
    public float Timer;
    public float Duration;
    public bool IsActive;
    public Sprite Image;

    [Header("Equip Settings")]
    public bool shouldEquip;
    public bool shouldShowInUI;

    [Header("Level")]
    public int maxLevel = 5;
    public int currentLevel = 1;
    public float defUpgradeCost = 500;

   

    public void LevelUp()
    {
        Duration += currentLevel * 1.5f;
    }
}


public enum PowerUpType
{
    PowerBoots,
    ScoreMultiplier,
    Magnet,
    Skate
}
