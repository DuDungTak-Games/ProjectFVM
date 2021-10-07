using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TileSetData", menuName = "Scriptable Object/TileSetData")]
public class TileSetData : ScriptableObject
{
    [SerializeField]
    public SerializeDictionary<TileID, SubList<TileSet>> tileSetList;
    public List<GimicSet> gimicSetList;
}