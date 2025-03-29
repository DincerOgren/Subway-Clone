using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasCloser : MonoBehaviour
{
    public GameObject[] canvasToClose;



    private void Start()
    {
        // we dont want hoverboard in start 
        for (int i = 0; i < canvasToClose.Length; i++)
        {
            canvasToClose[i].SetActive(false);
        }
    }
}
