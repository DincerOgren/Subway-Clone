using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    [SerializeField] float rotateSpeed = 120f;
    [SerializeField] float scorePointsPerCoin = 20f;

    private void Update()
    {
        transform.Rotate(0, rotateSpeed * Time.deltaTime, 0);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            
            //Maybe particle
            // Sound fx
            PlayerCollectibleManager.instance.AddCoin();
            ScoreManager.instance.AddScore(scorePointsPerCoin);
            gameObject.SetActive(false);

        }
    }
}
