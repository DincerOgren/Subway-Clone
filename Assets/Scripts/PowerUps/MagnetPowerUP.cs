using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagnetPowerUP : MonoBehaviour
{
    [SerializeField] float _magnetRadius = 10f;
    [SerializeField] LayerMask coinLayer;

    [SerializeField] PowerUp magnetData;
    public Transform magnetPoint;
    private void Update()
    {
        if (magnetData.IsActive)
        {

            GatherCoins();
        }
    }

    private void GatherCoins()
    {
        print("Gathering Coins");
        var collider = Physics.OverlapSphere(magnetPoint.position, _magnetRadius, coinLayer);
        foreach (var item in collider)
        {
            print("item name = " + item.name);
            item.GetComponent<Coin>().shouldMoveToMagnet = true;
            print("item true yapildi");
        }
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(magnetPoint.position, _magnetRadius);
    }
}
