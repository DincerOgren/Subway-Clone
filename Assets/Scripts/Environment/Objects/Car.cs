using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car : MonoBehaviour
{
    public float trainOffset = 8f;
    public float speed=5f;
    // Start is called before the first frame update
 
    private void OnEnable()
    {
        speed = GameObject.FindWithTag("Player").transform.GetComponent<PlayerMovement>().GetSpeed();
        trainOffset = speed * 4f;
        transform.position += Vector3.forward * trainOffset;

    }
    // Update is called once per frame
    void Update()
    {
        transform.Translate(0,0,speed * Time.deltaTime);

    }
}
