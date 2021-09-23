using UnityEngine;

[CreateAssetMenu(fileName = "TileFloorPrefabData", menuName = "Scriptable Object/TileFloorPrefabData")]
public class TileFloorPrefabData : ScriptableObject
{
    public SerializeDictionary<TileFloorID, GameObject> prefabList;

    public GameObject GetPrefab(TileFloorID id)
    {
        if (prefabList.ContainsKey(id))
        {
            return prefabList[id];
        }
        
        return null;
    }
}
