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
        if (glowMaterial != null)
        {
            rend.material = glowMaterial;
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
            buildManager.Selling = true;
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
