using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnTrigger : MonoBehaviour
{
    public Transform objectSpawnLoc;
    public Transform objectSpawnLoc2;
    public ObjectList objectList;
    public Transform spawnedObjectParent;
    private void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("Player"))
        {
            int randomNum = GetRandom(3);
            if (randomNum == 1)
            {

                var objec = Instantiate(objectList.GetRandomObject(), objectSpawnLoc.position, Quaternion.identity);
                objec.transform.parent = spawnedObjectParent;

            }
            else if (randomNum == 2)
            {
                var objec = Instantiate(objectList.GetRandomGroundObject(), objectSpawnLoc.position + (Vector3.up /2), Quaternion.identity);
                objec.transform.parent = spawnedObjectParent;

            }
            else
            {
                print("bay gecti");
            }




            var trainObcejt = Instantiate(objectList.GetRandomGroundObject(), objectSpawnLoc2.position + (Vector3.up/2), Quaternion.identity);
            trainObcejt.transform.parent = spawnedObjectParent;






        }
    }


    private int GetRandom(int max)
    {
        return Random.Range(0, max);
    }
}
