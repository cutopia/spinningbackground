using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerModel : MonoBehaviour
{

    public static List<string> hamsterGenomes;
    
    public void SaveData()
    {
        PlayerPrefs.DeleteAll(); // woo! so crazy!
        for (int i = 0; i < hamsterGenomes.Count; i++)
        {
            PlayerPrefs.SetString("hamster_" + i, hamsterGenomes[i]);
        }
        PlayerPrefs.SetInt("hamsterCount", hamsterGenomes.Count);
        PlayerPrefs.Save();
    }

    static void LoadData()
    {
        hamsterGenomes = new List<string>();
        int hamsterCount = 0;
        try
        {
            hamsterCount = PlayerPrefs.GetInt("hamsterCount");
        } catch (PlayerPrefsException e)
        {
            // this is going to happen if the player is brand new.
            Debug.Log(e.GetType() + ": " + e.Message);
            return;
        }
        if (hamsterCount > 0)
        {
            for (int i = 0; i < hamsterCount; i++)
            {
                hamsterGenomes.Add(PlayerPrefs.GetString("hamster_" + i));
            }
        }
    }
    
    
    // Ds = dominant spot. fatal when homozygous
    // Ba = banded. white band around middle of body
    // Whwh = roan WhWh would be eyeless white babies
    // rdrd = recessive dappled. white body with colored white blazed head and butt
}
