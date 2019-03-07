using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

public class SelectAllOfTagScriptableWizard : ScriptableWizard
{
    public string searchTag = "Your tag here";
    [MenuItem("My Tools/Select all of tag...")]
    static void SelectAllOfTagWizard()
    {
        ScriptableWizard.DisplayWizard<SelectAllOfTagScriptableWizard>("Select all of tag...", "Make Selection");
    }
    
    void OnWizardCreate()
    {
        GameObject[] gameObjects = GameObject.FindGameObjectsWithTag(searchTag);
        Selection.objects = gameObjects;
    }
}
