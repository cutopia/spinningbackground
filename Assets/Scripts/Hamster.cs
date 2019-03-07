using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hamster : MonoBehaviour
{
    [SerializeField] GameObject[] skinPrefabs;
    [SerializeField] GameObject[] genderSymbols;
    public string visibleGenome;
    public string gender;
    public string wholeGenome;
    
    /// <summary>
    /// genome comes in from saved data as
    /// a concatenation divided with "_" of 
    /// gender_visiblehamstergenes_totalgenome
    /// </summary>
    /// <param name="genome">Genome.</param>
    public void SetGenome(string genome)
    {
        string[] segments = genome.Split('_');
        if (segments.Length != 3)
        {
            Debug.Log("FAIL: Error in genome: " + genome);
            return;
        }
        gender = segments[0];
        visibleGenome = segments[1];
        wholeGenome = segments[2];
        foreach (var skinType in skinPrefabs)
        {
            Debug.Log(skinType.name);
            if (skinType.name == visibleGenome)
            {
                GameObject skin = Instantiate(skinType) as GameObject;
                skin.transform.SetParent(transform);
            }
            
        }
        foreach (var symbol in genderSymbols)
        {
            if (symbol.name == gender)
            {
                GameObject genderIcon = Instantiate(symbol) as GameObject;
                genderIcon.transform.SetParent(transform);
            }
        }
    }
}
