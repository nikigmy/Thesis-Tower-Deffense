using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class GrassTile : Tile
{
    Tower currentTower;
    [SerializeField]
    Material glowMaterial;
    Material normalMaterial;

    MeshRenderer rend;

    bool isMouseIn;
    bool wasMouseIn;
    // Use this for initialization
    void Start()
    {
        rend = GetComponent<MeshRenderer>();
        normalMaterial = rend.material;
        isMouseIn = false;
        wasMouseIn = false;
    }

    private void OnMouseOver()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            isMouseIn = false;
            return;
        }
        isMouseIn = true;
    }

    private void HandleMouseEnter()
    {
        var buildManager = GameManager.instance.BuildManager;
        if (glowMaterial != null)
        {
            rend.material = glowMaterial;
            if (lifter != null)
            {
                lifter.SetMaterial(glowMaterial);
            }
        }
        if (currentTower == null &&
            buildManager.CurrentTower != null && buildManager.Building &&
            GameManager.instance.Money >= buildManager.CurrentTower.CurrentPrice)
        {
            BuildTower(buildManager);
        }
        else if (currentTower != null && buildManager.Selling)
        {
            DestroyTower(buildManager);

        }
        if (currentTower != null)
        {
            currentTower.GetComponent<Tower>().EnableRangeGizmo();
        }
    }

    public void OnMouseDown()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        var buildManager = GameManager.instance.BuildManager;
        if (currentTower == null && buildManager.CurrentTower != null &&
            GameManager.instance.Money >= buildManager.CurrentTower.CurrentPrice)
        {
            BuildTower(buildManager);
            currentTower.GetComponent<Tower>().EnableRangeGizmo();
            if (Def.Instance.Settings.FastBuilding)
            {
                buildManager.Building = true;
            }
        }
        else if (currentTower != null && buildManager.SellClicked)
        {
            currentTower.GetComponent<Tower>().DisableRangeGizmo();
            DestroyTower(buildManager);
            if (Def.Instance.Settings.FastBuilding)
            {
                buildManager.Selling = true;
            }
        }
    }

    private void DestroyTower(BuildManager buildManager)
    {
        buildManager.DestroyTower(currentTower);
        currentTower = null;
    }

    private void BuildTower(BuildManager buildManager)
    {
        currentTower = buildManager.BuildTower(this);
    }

    private void OnMouseExit()
    {
        isMouseIn = false;
    }

    private void HandleMouseExit()
    {
        if (rend.material != normalMaterial)
        {
            rend.material = normalMaterial;
            if (lifter != null)
            {
                lifter.SetMaterial(normalMaterial);
            }
        }
        if (currentTower != null)
        {
            currentTower.GetComponent<Tower>().DisableRangeGizmo();
        }
    }

    private void LateUpdate()
    {
        bool groupedMouseIn;
        if(lifter != null)
        {
            groupedMouseIn = isMouseIn || lifter.IsMouseIn;
        }
        else
        {
            groupedMouseIn = isMouseIn;
        }
        if(groupedMouseIn && !wasMouseIn)
        {
            HandleMouseEnter();
        }
        else if(!groupedMouseIn && wasMouseIn)
        {
            HandleMouseExit();
        }
        wasMouseIn = groupedMouseIn;
    }
}
