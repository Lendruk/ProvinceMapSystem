using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ProvinceMaker))]
public class ProvinceMakerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        ProvinceMaker script = (ProvinceMaker)target;
        if(GUILayout.Button("Save Countries"))
        {
            if (Application.isPlaying)
                script.SaveCountries();
        }
        if(GUILayout.Button("Save Provinces"))
        {
            if(Application.isPlaying)
                script.SaveProvinces();
        }
        
    }
}
