using System;
using System.Collections.Generic;
using UnityEngine;

using DuDungTakGames.Extensions;

public class TileManager : MonoBehaviour
{

    public TilePrefabData prefabData;
    public TileFloorPrefabData floorPrefabData;
    public TileSetData mainTileSetData;
    public TileSetData subTileSetData;

    private List<Tuple<Vector3, GameObject>> spawnList = new List<Tuple<Vector3, GameObject>>();
    
    Transform rootTrf;

    void Awake()
    {
        Init();
    }

    void Init()
    {
        GameObject tileFolder = new GameObject("Tile Folder");
        rootTrf = tileFolder.transform;

        SpawnTileSet(mainTileSetData.tileSetList);
        SpawnTileSet(subTileSetData.tileSetList);
    }

    void SpawnTileSet(SerializeDictionary<TileID, SubList<TileSet>> tileSetList)
    {
        foreach (var data in tileSetList)
        {
            foreach (var tileSet in data.Value)
            {
                SpawnTile(data.Key, tileSet);
            }
        }
    }

    void SpawnTile(TileID tileID, TileSet tileSet)
    {
        GameObject prefab = GetTilePrefab(tileID, tileSet);
        GameObject tile = Instantiate(prefab, tileSet.spawnPos, Quaternion.Euler(tileSet.spawnRot), rootTrf);
        
        spawnList.Add(new Tuple<Vector3, GameObject>(tileSet.spawnPos, tile));

        float spawnFloor = tileSet.spawnFloor;
        switch (tileID)
        {
            case TileID.START_POINT:
                Transform playerTrf = FindObjectOfType<Player>().transform;
                playerTrf.SetPosition(tileSet.spawnPos);
                break;
            default:
                break;
        }

        BoxCollider boxCollider;
        if (tile.TryGetComponent(out boxCollider))
        {
            Floor tileFloor = tile.AddComponent<Floor>();
            
            tileFloor.floor = spawnFloor;
            tileFloor.tilePos = tileSet.tilePos; // NOTE : DEBUG ONLY
        }
    }
    
    GameObject GetTilePrefab(TileID tileID, TileSet tileSet)
    {
        switch (tileID)
        {
            case TileID.TILE:
                bool hasTop = FindTopTile(tileSet.spawnPos);
                TileFloorID floorID = hasTop ? TileFloorID.BOTTOM_TILE : TileFloorID.TOP_TILE;
                
                if (tileSet.spawnFloor % 1 != 0)
                {
                    floorID = hasTop ? TileFloorID.BOTTOM_HALF_TILE : TileFloorID.TOP_HALF_TILE;
                }

                return floorPrefabData.GetPrefab(floorID);
            default:
                return prefabData.GetPrefab(tileID);
        }
    }
    
    bool FindTopTile(Vector3 tilePos)
    {
        var data = spawnList.Find(x =>
            x.Item1 == tilePos + (Vector3.up * 7.5f) || 
            x.Item1 == tilePos + (Vector3.up * 10));

        return (data != null);
    }
}
