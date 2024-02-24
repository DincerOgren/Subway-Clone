using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneGenerator : MonoBehaviour
{
    [Tooltip("Prefab to generate for floor")]
    [SerializeField] GameObject planePrefab;

    public Transform planeParents;

   public List<GameObject> planePool = new();

    private void OnEnable()
    {
        Actions.GeneratePlane += GeneratePlane;
    }
    private void OnDisable()
    {
        Actions.GeneratePlane -= GeneratePlane;
    }

    public GameObject GeneratePlane(float v)
    {
        for (int i = 0; i < planePool.Count; i++)
        {
            if (planePool[i] != null && !planePool[i].activeSelf)
            {
                planePool[i].transform.position = new Vector3(0, 0, v);
                planePool[i].gameObject.SetActive(true);
                return planePool[i];
            }
        }
        planePool.Add(Instantiate(planePrefab, new Vector3(0, 0, v),Quaternion.identity, planeParents));

        return planePool[^1];
    }
}
