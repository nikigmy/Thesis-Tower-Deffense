using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelect : MonoBehaviour
{
    [Header("Prefab Setup")]
    [SerializeField]
    Sprite NavigatorNonHighlighted;
    [SerializeField]
    Sprite NavigatorHighlighted;
    [SerializeField]
    Sprite NavigatorWithLinkNonHighlighted;
    [SerializeField]
    Sprite NavigatorWithLinkHighlighted;

    [SerializeField]
    GameObject StarPrefab;
    [SerializeField]
    GameObject StarredLevelPrefab;
    [SerializeField]
    GameObject DefaultLevelPrefab;
    [SerializeField]
    GameObject LockedLevelPrefab;
    
    [Header("Scene Setup")]
    [SerializeField]
    Transform LevelsContent;
    [SerializeField]
    GameObject UnlockedLevelPreview;
    [SerializeField]
    GameObject LockedLevelPreview;
    [SerializeField]
    Transform LevelNavigatorContent;

    private CanvasGroup levelSelectCanvasGroup;
    private const int CellsPerPage = 10;
    private int currentLevelPreviewIndex;
    private int currentPageIndex;
    private void Start()
    {
        levelSelectCanvasGroup = GetComponent<CanvasGroup>();
    }

    public void Load()
    {
        LoadPage(0);
    }

    void LoadPage(int index)
    {
        if (Def.Instance.Levels.Count > index * CellsPerPage)
        {
            DestroyChildrenOfTransform(LevelsContent);

            for (int i = index * CellsPerPage; i < (index + 1) * CellsPerPage && i < Def.Instance.Levels.Count; i++)
            {
                int levelIndex = i;
                var levelData = Def.Instance.Levels[levelIndex];
                GameObject level;
                if (levelData.Unlocked || GameManager.instance.DeveloperMode)
                {
                    if (levelData.Stars > 0)
                    {
                        level = Instantiate(StarredLevelPrefab, LevelsContent);
                        var starContent = level.transform.GetChild(1);
                        for (int j = 0; j < levelData.Stars; j++)
                        {
                            Instantiate(StarPrefab, starContent);
                        }
                    }
                    else
                    {
                        level = Instantiate(DefaultLevelPrefab, LevelsContent);
                    }
                    level.transform.GetChild(0).GetComponent<Text>().text = (levelIndex + 1).ToString();
                }
                else
                {
                    level = Instantiate(LockedLevelPrefab, LevelsContent);
                }
                level.GetComponent<Button>().onClick.AddListener(delegate { LevelClicked(levelIndex); });
            }

            GeneratePageNavigator(index);
            currentPageIndex = index;
        }
    }

    private void DestroyChildrenOfTransform(Transform transform)
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            Destroy(transform.GetChild(i).gameObject);
        }
    }

    private void GeneratePageNavigator(int index)
    {
        var totalNumberOfPages = (int)Math.Round(Def.Instance.Levels.Count / (float)CellsPerPage, MidpointRounding.AwayFromZero);
        if (index >= 0 && index <= totalNumberOfPages)
        {
            DestroyChildrenOfTransform(LevelNavigatorContent);
            var navigatorPart = new GameObject();
            var image = navigatorPart.AddComponent<Image>();
            for (int i = 0; i < totalNumberOfPages; i++)
            {
                Sprite spriteForPart;
                if(index == i)
                {
                    if (i == totalNumberOfPages - 1)
                    {
                        spriteForPart = NavigatorHighlighted;
                    }
                    else
                    {
                        spriteForPart = NavigatorWithLinkHighlighted;
                    }
                }
                else
                {
                    if (i == totalNumberOfPages - 1)
                    {
                        spriteForPart = NavigatorNonHighlighted;
                    }
                    else
                    {
                        spriteForPart = NavigatorWithLinkNonHighlighted;
                    }
                }
                image.sprite = spriteForPart;
                Instantiate(navigatorPart, LevelNavigatorContent);
            }
        }
    }

    public void PlayLevelClicked()
    {
        levelSelectCanvasGroup.blocksRaycasts = true;
        levelSelectCanvasGroup.alpha = 1;
        levelSelectCanvasGroup.interactable = true;
        UnlockedLevelPreview.SetActive(false);

        GameManager.instance.LoadLevel(currentLevelPreviewIndex);
    }

    public void LevelClicked(int index)
    {
        levelSelectCanvasGroup.blocksRaycasts = false;
        levelSelectCanvasGroup.alpha = 0.5f;
        levelSelectCanvasGroup.interactable = false;

        currentLevelPreviewIndex = index;
        var levelData = Def.Instance.Levels[index];
        if (levelData.Unlocked || GameManager.instance.DeveloperMode)
        {
            UnlockedLevelPreview.SetActive(true);
            UnlockedLevelPreview.transform.GetChild(1).GetChild(0).GetComponent<Text>().text = levelData.Name;
            UnlockedLevelPreview.transform.GetChild(2).GetChild(0).GetComponent<Image>().sprite = levelData.previewSprite;
        }
        else
        {
            LockedLevelPreview.SetActive(true);
            LockedLevelPreview.transform.GetChild(1).GetChild(0).GetComponent<Text>().text = levelData.Name;
        }
    }

    public void ChangePage(bool right)
    {
        var dir = 0;
        if (right)
        {
            dir = 1;
        }
        else
        {
            dir = -1;
        }
        var pageToSwitchTo = currentPageIndex + dir;
        float totalNumberOfPages = Def.Instance.Levels.Count / (float)CellsPerPage;
        if (pageToSwitchTo >= 0 && pageToSwitchTo <= totalNumberOfPages)
        {
            LoadPage(pageToSwitchTo);
        }
    }
}
