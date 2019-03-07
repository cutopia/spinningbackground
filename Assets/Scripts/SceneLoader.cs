using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public void LoadHamsterGladeScene()
    {
        if (SceneManager.GetActiveScene().name != "HamsterGlade")
        {
            SceneManager.LoadScene("HamsterGlade");
        }
    }

    public void LoadCorralScene()
    {
        if (SceneManager.GetActiveScene().name != "Corral")
        {
            SceneManager.LoadScene("Corral");
        }
    }

    public void LoadQuestScene()
    {
        if (SceneManager.GetActiveScene().name != "Quests")
        {
            SceneManager.LoadScene("Quests");
        }
    }

    public void LoadMainMenuScene()
    {
        if (SceneManager.GetActiveScene().name != "MainMenu")
        {
            SceneManager.LoadScene("MainMenu");
        }
    }
}
