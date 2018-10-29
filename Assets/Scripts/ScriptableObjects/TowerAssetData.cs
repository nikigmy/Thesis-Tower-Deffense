using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class TowerAssetData : ScriptableObject
{
    public string Description;

    public Declarations.TowerType Type;
    
    public Sprite Level1Sprite;
    public Sprite Level2Sprite;
    public Sprite Level3Sprite;

    public GameObject Prefab;
}
