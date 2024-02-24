using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    public float speed = 1f;
    public float t;
    public Transform sphere;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        t = (Mathf.Sin(Time.time * speed)+1)/2;
        print(t);
    }
}
