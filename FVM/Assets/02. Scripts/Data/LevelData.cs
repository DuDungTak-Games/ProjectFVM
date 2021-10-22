using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelData", menuName = "Scriptable Object/LevelData")]
public class LevelData : ScriptableObject
{
    [SerializeField]
    public TileSetData mainPreset;
    
    [SerializeField]
    public List<TileSetData> subPreset;
}