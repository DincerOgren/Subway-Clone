using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class SavingWrapper : MonoBehaviour
{
    const string defaultSaveFile = "save";


    private void Start()
    {
        //Load();
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            Save();
            print("Saved");
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            Load();
            print("Loaded");

        }

    }


    public void Save()
    {
        GetComponent<SavingSystem>().Save(defaultSaveFile);
    }

    public void Load()
    {
        GetComponent<SavingSystem>().Load(defaultSaveFile);

    }
}
