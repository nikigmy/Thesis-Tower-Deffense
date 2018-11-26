using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour {

    [SerializeField]
    GameObject levelSelectButton;
    [SerializeField]
    GameObject levelEditorButton;
    [SerializeField]
    GameObject quitButton;
    [SerializeField]
    GameObject levelSelectPanel;
    [SerializeField]
    GameObject levelSelectBack;
    [SerializeField]
    GameObject levelRowPrefab;
    [SerializeField]
    GameObject levelPrefab;
    
    public void LoadLevelPreviews()
    {
        var content = levelSelectPanel.transform.GetChild(0).GetChild(0).GetChild(0);
        GameObject currentLevelRow = null;
        for (int i = 0; i < Def.Instance.Levels.Length; i++)
        {
            int levelIndex = i;
            if (levelIndex % 3 == 0)
            {
                currentLevelRow = Instantiate(levelRowPrefab, content);
            }
            var level = Instantiate(levelPrefab, currentLevelRow.transform);
            level.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(delegate { GameManager.instance.LoadLevel(levelIndex); });
            level.transform.GetChild(0).GetComponent<UnityEngine.UI.Image>().sprite = Def.Instance.Levels[levelIndex].previewSprite;
        }
    }

    public void LevelCreatorClicked()
    {
        SceneManager.LoadScene("LevelCreator");
    }

    public void SwitchPanels(bool toLevelSelect)
    {
        if (toLevelSelect)
        {
            levelSelectPanel.SetActive(true);
            levelSelectBack.SetActive(true);

            levelSelectButton.SetActive(false);
            levelEditorButton.SetActive(false);
            quitButton.SetActive(false);
        }
        else
        {
            levelSelectPanel.SetActive(false);
            levelSelectBack.SetActive(false);

            levelEditorButton.SetActive(true);
            levelSelectButton.SetActive(true);
            quitButton.SetActive(true);

        }
    }

    public void Quit()
    {
        Application.Quit();
    }
}
