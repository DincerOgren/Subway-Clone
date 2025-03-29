using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plane : MonoBehaviour
{

    float planeOffset;

   

    private void OnEnable()
    {
        planeOffset = (10 * transform.localScale.z) + transform.localPosition.z;
    }

    public void ResetPlaneOffset()
    {
        planeOffset = (10 * transform.localScale.z) + transform.localPosition.z;
    }

    private void OnTriggerEnter(Collider other)
    {
        
        if (other.CompareTag("Player"))
        {
            
            Actions.GeneratePlane?.Invoke(planeOffset);
        }

        if(other.CompareTag("PlaneDeactivator"))
        {
            gameObject.SetActive(false);
        }
    }
}
