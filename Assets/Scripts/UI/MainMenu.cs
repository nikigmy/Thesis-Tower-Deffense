using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour {

    [SerializeField]
    GameObject levelSelectButton;
    [SerializeField]
    GameObject quitButton;
    [SerializeField]
    GameObject LevelSelectPanel;
    [SerializeField]
    GameObject levelRowPrefab;
    [SerializeField]
    GameObject levelPrefab;

    public void OnLevelSelectClicked()
    {
        var content = LevelSelectPanel.transform.GetChild(0).GetChild(0).GetChild(0);
        GameObject currentLevelRow = null;
        for (int i = 0; i < Def.Instance.Levels.Length; i++)
        {
            if(i % 3 == 0)
            {
                currentLevelRow = Instantiate(levelRowPrefab, content);
            }
            var level = Instantiate(levelPrefab, currentLevelRow.transform);
            level.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(delegate { GameManager.instance.LoadLevel(i); });
            level.GetComponent<UnityEngine.UI.Image>().sprite = Def.Instance.Levels[i].previewSprite;
        }
    }

    void OnPlayLevelClicked()
    {

    }

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    internal void LoadLevelSelector()
    {
        throw new NotImplementedException();
    }
}
