using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class EditableTile : Tile
{

    MeshRenderer rend;
    // Use this for initialization
    void Awake()
    {
        rend = GetComponent<MeshRenderer>();
        GameManager.instance.PaintManager.UpdateTiles.AddListener(UpdateTile);
    }

    public override void SetData(int row, int col, Declarations.TileType type)
    {
        base.SetData(row, col, type);
        rend.material = GameManager.instance.MapGenerator.TileMaterials[type];
    }

    private void OnMouseEnter()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        var paintManager = GameManager.instance.PaintManager;
        if (paintManager.Painting)
        {
            ChangeTile(paintManager);
        }
        if (paintManager.BrushSize > 1)
        {
            if (paintManager.Painting)
            {
                foreach (var cell in Helpers.GetTilesInRange(transform.position, paintManager.BrushSize).Where(x => Vector3.Distance(x.transform.position, transform.position) <= paintManager.BrushSize))
                {
                    ((EditableTile)cell).ChangeTile(paintManager);
                }
            }
            paintManager.ChangeMousePos(new Declarations.IntVector2(Col, Row));
        }
        else
        {
            rend.material = GameManager.instance.MapGenerator.GlowMaterials[Type];
        }

    }

    private void OnMouseExit()
    {
        var paintManager = GameManager.instance.PaintManager;
        if (paintManager.BrushSize > 0)
        {
            paintManager.UpdateTiles.Invoke();
        }
        else
        {
            rend.material = GameManager.instance.MapGenerator.TileMaterials[Type];
        }
    }

    private void OnMouseDown()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        var paintManager = GameManager.instance.PaintManager;
        if (paintManager.CurrentTileType != Declarations.TileType.Unknown)
        {
            if (Def.Instance.Settings.FastBuilding)
            {
                paintManager.Painting = true;
            }
            ChangeTile(paintManager);
            if (paintManager.BrushSize > 1)
            {
                foreach (var cell in Helpers.GetTilesInRange(transform.position, paintManager.BrushSize).Where(x => Vector3.Distance(x.transform.position, transform.position) <= paintManager.BrushSize))
                {
                    ((EditableTile)cell).ChangeTile(paintManager);
                }
                paintManager.UpdateTiles.Invoke();
            }
            else
            {
                rend.material = GameManager.instance.MapGenerator.GlowMaterials[Type];
            }
        }
    }

    private void UpdateTile()
    {
        var paintManager = GameManager.instance.PaintManager;
        if (Vector3.Distance(Helpers.GetPositionForTile(paintManager.CurrectMousePos.y, paintManager.CurrectMousePos.x), transform.position) <= paintManager.BrushSize)
        {
            rend.material = GameManager.instance.MapGenerator.GlowMaterials[Type];
        }
        else
        {
            rend.material = GameManager.instance.MapGenerator.TileMaterials[Type];
        }
    }

    public void ChangeTile(PaintManager paintManager)
    {
        if (paintManager.Painting && paintManager.CurrentTileType != Declarations.TileType.Unknown && paintManager.CurrentTileType != Type)
        {
            Type = paintManager.CurrentTileType;
        }
    }
}
