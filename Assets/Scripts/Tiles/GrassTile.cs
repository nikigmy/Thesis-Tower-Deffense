using UnityEngine;
using UnityEngine.EventSystems;

public class GrassTile : Tile
{
    Tower currentTower;
    [SerializeField]
    Material glowMaterial;
    Material normalMaterial;

    MeshRenderer rend;
    // Use this for initialization
    void Start()
    {
        rend = GetComponent<MeshRenderer>();
        normalMaterial = rend.material;
    }

    private void OnMouseEnter()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        var buildManager = GameManager.instance.BuildManager;

        if (glowMaterial != null &&
            ((buildManager.CurrentTower != null &&
            GameManager.instance.Money >= buildManager.CurrentTower.CurrentPrice) ||
            buildManager.SellClicked))
        {
            if (buildManager.Building)
            {
                BuildTower(buildManager);
            }
            else if (buildManager.Selling)
            {
                DestroyTower(buildManager);
            }
            rend.material = glowMaterial;
        }
    }

    private void OnMouseDown()
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
            buildManager.Building = true;
        }
        else if (currentTower != null && buildManager.SellClicked)
        {
            DestroyTower(buildManager);
        }
    }

    private void DestroyTower(BuildManager buildManager)
    {
        var tower = currentTower;
        buildManager.DestroyTower(currentTower);
    }

    private void BuildTower(BuildManager buildManager)
    {
        currentTower = buildManager.BuildTower(this);
    }

    private void OnMouseExit()
    {
        if (rend.material != normalMaterial)
        {
            rend.material = normalMaterial;
        }
    }
}
