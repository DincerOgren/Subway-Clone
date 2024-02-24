using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectDestroyer : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("ObstacleGround") )
        {
                other.gameObject.SetActive(false);
        }
        if (other.CompareTag("Obstacle") || other.CompareTag("Coin"))
        {
            
                other.gameObject.SetActive(false);
        }

    }

   
}
