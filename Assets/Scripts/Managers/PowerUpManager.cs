using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpManager : MonoBehaviour
{

    public static PowerUpManager instance;

    [Header("PowerUp Datas")]
    public PowerUp[] allPowerUpDatas;

    [Header("PowerUp Spawn Points")]
    public Transform shoePointR;
    public Transform shoePointL;
    public Transform magnetPoint;

    [Header("PowerUp UI")]
    public Transform powerUpUIContainer;
    public PowerUpUI powerUpUIPrefab;


    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        if (instance!=this)
            Destroy(gameObject);


        DontDestroyOnLoad(gameObject);
    }


    public void EquipPrefab(PowerUp data)
    {
        if (data.equipPrefab!=null)
        {
            //Spawn
            if (data.powerUpType == PowerUpType.PowerBoots)
            {
                //    Instantiate(data.equipPrefab, shoePointR);
                //    var leftShoe = Instantiate(data.equipPrefab, shoePointL);
                //    leftShoe.transform.rotation = Quaternion.Euler(0, 0, 180);
                shoePointL.gameObject.SetActive(true);
                shoePointR.gameObject.SetActive(true);
                print("setaktiflesti?");
            }
            else if (data.powerUpType == PowerUpType.Magnet)
            {
                //Instantiate(data.equipPrefab, magnetPoint);
                magnetPoint.gameObject.SetActive(true);
            }
        }
    }

    public void TakePowerUp(PowerUp data)
    {
        if (data.IsActive)
        {
            data.Timer= 0;
        }
        else
        {
            var uiPrefab = Instantiate(powerUpUIPrefab, powerUpUIContainer);

            print(uiPrefab.name + " NAMEEE");

            uiPrefab.powerUpData = data;
            data.IsActive = true;

            data.Timer= 0;
            StartCoroutine(StartTimer(data));
        }
    }

    IEnumerator StartTimer(PowerUp data)
    {
        while (true)
        {
            if (data.Timer>=data.Duration)
            {
                break;
            }



            data.Timer += Time.deltaTime;
            yield return null;
        }

        ResetPowerUp(data);
    }

    private void ResetPowerUp(PowerUp data)
    {
        data.IsActive = false;
        data.Timer = Mathf.Infinity;

        if (data.equipPrefab != null)
        {
            if (data.powerUpType == PowerUpType.PowerBoots)
            {
                //    Instantiate(data.equipPrefab, shoePointR);
                //    var leftShoe = Instantiate(data.equipPrefab, shoePointL);
                //    leftShoe.transform.rotation = Quaternion.Euler(0, 0, 180);
                shoePointL.gameObject.SetActive(false);
                shoePointR.gameObject.SetActive(false);

            }
            else if (data.powerUpType == PowerUpType.Magnet)
            {
                //Instantiate(data.equipPrefab, magnetPoint);
                magnetPoint.gameObject.SetActive(false);
            }
        }
    }


    public PowerUp GetPowerUpData(PowerUpType type)
    {
        for (int i = 0; i < allPowerUpDatas.Length; i++)
        {
            if (allPowerUpDatas[i].powerUpType == type)
            {
                return allPowerUpDatas[i];
                
            }
        }
        
        return null;
    }
}