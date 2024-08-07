using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ObjectList", menuName = "ScriptableObjects/List", order = 1)]
public class ObjectList : ScriptableObject
{

    public List<GameObject> groundObjectPrefabs;
    public List<GameObject> objectPrefabs;


    public GameObject GetRandomObject()
    {
        return objectPrefabs[Random.Range(0,objectPrefabs.Count)];
    }

    public GameObject GetRandomGroundObject()
    {
        return groundObjectPrefabs[Random.Range(0,groundObjectPrefabs.Count)];
    }
}
