using System;
using UnityEngine;

public static class Actions
{

    public delegate GameObject OnGeneratePlane(float v);
    public static OnGeneratePlane GeneratePlane;


}