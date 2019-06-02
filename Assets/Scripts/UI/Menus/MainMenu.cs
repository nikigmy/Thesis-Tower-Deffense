using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField]
    GameObject LevelEditorButton;
    [SerializeField]
    LevelSelect levelSelect;

    public void SetButtonActivity()
    {
        var image = LevelEditorButton.GetComponent<Image>();
        var button = LevelEditorButton.GetComponent<Button>();
        if (GameManager.instance.DeveloperMode)
        {
            button.interactable = true;
            image.color = new Color(1, 1, 1, 1);
        }
        else
        {
            button.interactable = false;
            image.color = new Color(1, 1, 1, 0.5f);
        }
    }

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
        SetButtonActivity();
        levelSelect.ReloadPage();
    }
}
