using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lifter : MonoBehaviour {

    public Tile Tile { get; protected set; }
    public bool IsMouseIn;

    MeshRenderer rend;
    // Use this for initialization
    void Start () {
        rend = GetComponent<MeshRenderer>();
        IsMouseIn = false;
    }

    public void SetTile(Tile tileToSet)
    {
        Tile = tileToSet;
    }

    private void OnMouseDown()
    {
        if(Tile != null && Tile.Type == Declarations.TileType.Grass)
        {
            (Tile as GrassTile).OnMouseDown();
        }
    }
    private void OnMouseOver()
    {
        if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
        {
            IsMouseIn = false;
            return;
        }
        IsMouseIn = true;
    }

    private void OnMouseExit()
    {
        IsMouseIn = false;
    }

    internal void SetMaterial(Material material)
    {
        rend.material = material;
    }
}
