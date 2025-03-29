using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


[ExecuteAlways]
public class SaveableEntity : MonoBehaviour
{
    public string uniqeIdentifier = "";


    public string GetUniqeIdentifier()
    {
        return uniqeIdentifier;
    }

    public object CaptureState()
    {
        Dictionary<string, object> state = new();   
        foreach (ISaveable comp in GetComponents<ISaveable>())
        {
            state[comp.GetType().ToString()] = comp.CaptureState();
        }

        return state;
    }

    public void RestoreState(object state)
    {
        Dictionary<string, object> stateDict = (Dictionary<string, object>)state;

        foreach (ISaveable comp in GetComponents<ISaveable>())
        {
            string typeString = comp.GetType().ToString();
            if (stateDict.ContainsKey(typeString))
            {
                comp.RestoreState(stateDict[typeString]);
            }
        }

    }

#if UNITY_EDITOR
    private void Update()
    {
        if (Application.IsPlaying(gameObject)) return;

        if (string.IsNullOrEmpty(gameObject.scene.path)) return;

        SerializedObject serializedObject = new SerializedObject(this);
        SerializedProperty property = serializedObject.FindProperty("uniqeIdentifier");

        if (string.IsNullOrEmpty(property.stringValue))
        {
            property.stringValue = System.Guid.NewGuid().ToString();
            serializedObject.ApplyModifiedProperties();

        }
    }
#endif
}
