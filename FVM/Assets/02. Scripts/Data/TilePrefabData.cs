using UnityEngine;

[CreateAssetMenu(fileName = "TilePrefabData", menuName = "Scriptable Object/TilePrefabData")]
public class TilePrefabData : ScriptableObject
{
    public SerializeDictionary<TileID, GameObject> prefabList;
    public SerializeDictionary<TileID, float> floorUnitList;
    
    public GameObject GetPrefab(TileID id)
    {
        if (prefabList.ContainsKey(id))
        {
            return prefabList[id];
        }
        
        return null;
    }
    
    public float GetUnit(TileID id)
    {
        if (floorUnitList.ContainsKey(id))
        {
            return floorUnitList[id];
        }
        
        return 0;
    }
}
