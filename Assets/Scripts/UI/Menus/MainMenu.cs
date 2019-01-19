using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public void LevelCreatorClicked()
    {
        SceneManager.LoadScene("LevelCreator");
    }

    public void Quit()
    {
        Application.Quit();
    }
    
    public void DevModeClicked()
    {
        Debug.Log("Dev mode switched to " + !GameManager.instance.DeveloperMode);
        GameManager.instance.SetDevMode(!GameManager.instance.DeveloperMode);
    }
}
