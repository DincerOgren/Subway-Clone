using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    [SerializeField] float rotateSpeed = 120f;
    [SerializeField] float scorePointsPerCoin = 20f;


    [SerializeField] float _moveSpeedToMagnet = 15f;

    public bool shouldMoveToMagnet = false;


    Vector3 _targetPos;

    MagnetPowerUP player;
    private void Awake()
    {
        player = GameObject.FindWithTag("Player").GetComponent<MagnetPowerUP>();
    }
    private void OnEnable()
    {
       

    }

    private void Update()
    {
        transform.Rotate(0, rotateSpeed * Time.deltaTime, 0);

        if (shouldMoveToMagnet)
        {
            MoveToMagnet();
        }
    }

    void MoveToMagnet()
    {
        _targetPos = player.magnetPoint.position;
        transform.position = Vector3.MoveTowards(transform.position, _targetPos, _moveSpeedToMagnet * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Magnet"))
        {

            //Maybe particle
            // Sound fx
            PlayerCollectibleManager.instance.AddToCurrentCoin();
            ScoreManager.instance.AddScore(scorePointsPerCoin);
            shouldMoveToMagnet = false;
            gameObject.SetActive(false);

        }
    }
}
