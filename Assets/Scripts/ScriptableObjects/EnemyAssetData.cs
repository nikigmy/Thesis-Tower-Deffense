using UnityEngine;

[CreateAssetMenu()]
public class EnemyAssetData : ScriptableObject
{
    public string Description;

    public Declarations.EnemyType Type;
    
    public GameObject Prefab;
}
