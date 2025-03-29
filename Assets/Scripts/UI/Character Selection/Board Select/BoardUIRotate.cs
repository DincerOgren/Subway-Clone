using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardUIRotate : MonoBehaviour
{
    public float rotateSpeed;

    private void Update()
    {
        if (transform.eulerAngles.y>=360f)
        {
            Vector3 temp= transform.eulerAngles;
            temp.y = 0;
            transform.eulerAngles = temp;
        }
        transform.Rotate(0, rotateSpeed * Time.deltaTime, 0);
    }
}
