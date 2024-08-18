using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneralPowerUp : MonoBehaviour
{
    public float rotateSpeed = 55f;

    public float waveSpeed = 3f;

    public float waveHeight = 0.005f;


    public PowerUp powerUpData;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {

            // TODO: EQUIP METHOD
            print("Equipped Prefab before");
            PowerUpManager.instance.EquipPrefab(powerUpData);
            print("Equipped Prefab after");

            PowerUpManager.instance.TakePowerUp(powerUpData);



            gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        StartAnimation();
    }


    void StartAnimation()
    {
        Vector3 temp = transform.position;

        temp.y += Mathf.Sin(waveSpeed * Time.time) * waveHeight;



        transform.position = temp;



        transform.Rotate(Vector3.up, -rotateSpeed * Time.deltaTime);

        if (transform.rotation.y == 720f || transform.rotation.y == -720f)
        {
            transform.rotation = Quaternion.Euler(transform.rotation.x, 0,transform.rotation.z);
        }

    }
}
