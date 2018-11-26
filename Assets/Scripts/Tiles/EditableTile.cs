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
        if (paintManager.LargeBrush)
        {
            if (paintManager.Painting)
            {
                foreach (var cell in GameManager.instance.MapGenerator.GetNeibourCells(Row, Col))
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
        if (paintManager.LargeBrush)
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
            paintManager.Painting = true;
            ChangeTile(paintManager);
            if (paintManager.LargeBrush)
            {
                foreach (var cell in GameManager.instance.MapGenerator.GetNeibourCells(Row, Col))
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
        if ((paintManager.CurrectMousePos.x == Col && paintManager.CurrectMousePos.y == Row) || (paintManager.LargeBrush &&
            GameManager.instance.MapGenerator.GetNeibourCells(Row, Col)
            .Any(x => x.Row == paintManager.CurrectMousePos.y && x.Col == paintManager.CurrectMousePos.x)))
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
