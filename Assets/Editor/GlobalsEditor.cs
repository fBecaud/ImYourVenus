using NUnit.Framework.Constraints;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Globals))]
public class GlobalsEditor : Editor
{
    public /*override*/ void OnInspectorGUI()
    {

        Globals playerScript = (Globals)target;


        if (GUILayout.Button("Set Time Step Second"))
        {
            playerScript.SetTimeStepSecond();
        }
        if (GUILayout.Button("Set Time Step Minute"))
        {
            playerScript.SetTimeStepMinute();
        }
        if (GUILayout.Button("Set Time Step Hour"))
        {
            playerScript.SetTimeStepHour();
        }
        if (GUILayout.Button("Set Time Step Day"))
        {
            playerScript.SetTimeStepDay();
        }
        if (GUILayout.Button("Set Time Step Month"))
        {
            playerScript.SetTimeStepMonth();
        }
    }
}
