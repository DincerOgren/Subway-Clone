using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System;

public class SavingSystem : MonoBehaviour
{
    public static SavingSystem Instance;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }




    public void Save(string saveFile)
    {
        SaveFile(saveFile,CaptureState());
    }

    private void SaveFile(string saveFile,object state)
    {
        string path = GetPathFromSaveFile(saveFile);
        print("Saving to " + path);
        using (FileStream fs = File.Open(path, FileMode.Create))
        {

            BinaryFormatter formatter = new();
            formatter.Serialize(fs, state);
        }
    }

    public void Load(string saveFile)
    {
        RestoreState(LoadFile(saveFile));
    }

    private Dictionary<string, object> LoadFile(string saveFile)
    {
        string path = GetPathFromSaveFile(saveFile);
        print("Loading from " + path);

        using (FileStream fs = File.Open(path, FileMode.Open))
        {

            BinaryFormatter formatter = new();
            return (Dictionary<string, object>)formatter.Deserialize(fs);
        }
    }

    Dictionary<string, object> CaptureState()
    {
        Dictionary<string, object> state = new Dictionary<string, object>();

        foreach (SaveableEntity entity in FindObjectsOfType<SaveableEntity>())
        {
            state[entity.GetUniqeIdentifier()] = entity.CaptureState();
        }

        return state;
    }

    void RestoreState(Dictionary<string, object> state)
    {

        foreach (SaveableEntity entity in FindObjectsOfType<SaveableEntity>())
        {
            entity.RestoreState(state[entity.GetUniqeIdentifier()]);
        }

    }

    string GetPathFromSaveFile(string savFile)
    {
        return Path.Combine(Application.persistentDataPath, savFile);
    }
}
