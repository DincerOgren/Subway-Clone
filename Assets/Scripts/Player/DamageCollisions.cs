using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageCollisions : MonoBehaviour
{
    public float sphereRadius = .5f;
    public float distanceFromPlayer = 1f;
    public float yOffset = .5f;
    public LayerMask obstacleLayer;


    Collider[] colliders = new Collider[50];

    private void Update()
    {
        if (Flags.isTakingDamage)
        {
            Physics.OverlapSphereNonAlloc(transform.position + Vector3.forward * distanceFromPlayer + Vector3.up * yOffset, sphereRadius, colliders, obstacleLayer);

            foreach (Collider collider in colliders)
            {
                if (collider != null)
                    collider.isTrigger = true;
            }
        }
        
    }

    public void ClearArray()
    {
        foreach (Collider collider in colliders)
        {
            if (collider != null)
                collider.isTrigger = false;
        }

        Array.Clear(colliders, 0, colliders.Length);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position + Vector3.forward * distanceFromPlayer + Vector3.up * yOffset, sphereRadius);
    }
}
