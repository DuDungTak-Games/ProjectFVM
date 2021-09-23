using UnityEngine;

[CreateAssetMenu(fileName = "TilePrefabData", menuName = "Scriptable Object/TilePrefabData")]
public class TilePrefabData : ScriptableObject
{
    public SerializeDictionary<TileID, GameObject> prefabList;
    public SerializeDictionary<TileID, float> floorUnitList;
}
