using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Level : MonoBehaviour {

    [SerializeField]
    Text NameText;
    [SerializeField]
    Image[] Stars;
    [SerializeField]
    Image Preview;
    [SerializeField]
    Button Button;
    [SerializeField]
    Outline Outline;

    [SerializeField]
    Sprite FullStar;
    [SerializeField]
    Sprite EmptyStar;

    public void SetData(Declarations.LevelData data)
    {
        NameText.text = data.Name;
        for (int i = 0; i < 3; i++)
        {
            if(i + 1 <= data.Stars)
            {
                Stars[i].sprite = FullStar;
            }
            else
            {
                Stars[i].sprite = EmptyStar;
            }
        }
        Preview.sprite = data.previewSprite;
        if (data.Unlocked || GameManager.instance.DeveloperMode)
        {
            Outline.effectColor = Color.green;
            Button.interactable = true;
        }
        else
        {
            Outline.effectColor = Color.red;
            Button.interactable = false;
        }
    }
}
