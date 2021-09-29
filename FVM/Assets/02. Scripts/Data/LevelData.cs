using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelData", menuName = "Scriptable Object/LevelData")]
public class LevelData : ScriptableObject
{
    [SerializeField]
    public TilePrefabData tilePrefabData;

    [SerializeField]
    public TileFloorPrefabData tileFloorPrefabData;

    [SerializeField]
    public TileSetData mainPreset;
    
    [SerializeField]
    public List<TileSetData> subPreset;
}