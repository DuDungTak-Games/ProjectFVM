using UnityEngine;

[CreateAssetMenu(fileName = "TilePrefabData", menuName = "Scriptable Object/TilePrefabData")]
public class TilePrefabData : ScriptableObject
{
    public SerializeDictionary<TileID, GameObject> prefabList = new SerializeDictionary<TileID, GameObject>();
    public SerializeDictionary<TileID, Vector3> offsetList = new SerializeDictionary<TileID, Vector3>();
    
    public GameObject GetPrefab(TileID id)
    {
        if (prefabList.ContainsKey(id))
        {
            return prefabList[id];
        }
        
        return null;
    }
    
    public Vector3 GetOffset(TileID id)
    {
        if (offsetList.ContainsKey(id))
        {
            return offsetList[id];
        }
        
        return Vector3.zero;
    }
}
