using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TileSetData", menuName = "Scriptable Object/TileSetData")]
public class TileSetData : ScriptableObject
{
    public SerializeDictionary<TileID, List<GameObject>> tileSetList;
}