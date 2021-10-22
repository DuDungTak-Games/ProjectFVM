using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ThemeData", menuName = "Scriptable Object/ThemeData")]
public class ThemeData : ScriptableObject
{
    [SerializeField]
    public TilePrefabData tilePrefabData;

    [SerializeField]
    public TileFloorPrefabData tileFloorPrefabData;
}