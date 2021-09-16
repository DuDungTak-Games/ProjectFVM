using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelData", menuName = "Scriptable Object Asset/LevelData")]
public class LevelData : ScriptableObject
{
    public int debugValue;
    
    public Dictionary<TileID, GameObject> prefabList;
    public Dictionary<TileID, float> floorUnitList;
    public Dictionary<TileID, List<TileSet>> tileSetList;
    public Dictionary<TileID, List<GameObject>> spawnList;
}
