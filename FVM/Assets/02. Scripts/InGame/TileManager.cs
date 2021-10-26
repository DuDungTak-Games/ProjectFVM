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

    // NOTE : TEST ONLY
    public GimicManager gimicManager;
    
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

        SpawnTileSet(mainTileSetData);
        SpawnTileSet(subTileSetData);

        gimicManager.Init();
    }

    void SpawnTileSet(TileSetData tileSetData)
    {
        foreach (var data in tileSetData.tileSetList)
        {
            foreach (var tileSet in data.Value)
            {
                SpawnTile(data.Key, tileSet, GetGimicSet(tileSetData.gimicSetList, tileSet.spawnPos));
            }
        }
    }

    void SpawnTile(TileID tileID, TileSet tileSet, GimicSet gimicSet)
    {
        GameObject prefab = GetTilePrefab(tileID, tileSet);
        GameObject tile = Instantiate(prefab, GetUnitTilePos(tileID, tileSet.spawnPos), 
                            Quaternion.Euler(tileSet.spawnRot), rootTrf);
        tile.name = tile.name.Replace("(Clone)", "").Trim();
        
        spawnList.Add(new Tuple<Vector3, GameObject>(tileSet.spawnPos, tile));

        float spawnFloor = tileSet.spawnFloor;
        switch (tileID)
        {
            case TileID.START_POINT:
                Transform playerTrf = FindObjectOfType<Player>().transform;
                playerTrf.SetPosition(tile.transform.position);
                playerTrf.SetRotation(tile.transform.rotation);
                break;
            case TileID.VM_POINT:
                GameObject triggerPrefab = GetTilePrefab(TileID.FOOTHOLD_TRIGGER, tileSet);
                Vector3 triggerPos = GetUnitTilePos(TileID.FOOTHOLD_TRIGGER, tileSet.spawnPos) + (tile.transform.forward.normalized * 10f);
                    
                GameObject triggerObj = Instantiate(triggerPrefab, triggerPos, Quaternion.Euler(tileSet.spawnRot), rootTrf);
                GimicTrigger trigger = triggerObj.GetComponent<GimicTrigger>();
                trigger.SetGimicID(-1);
                trigger.AddEvent(() => { TestGameManager.Instance.SetGameEvent(GameState.PREPARE); });
                break;
            default:
                break;
        }
        
        if ((int) tileID >= (int) TileID.FOOTHOLD_TRIGGER)
        {
            GimicObject gimic;
            if (tile.TryGetComponent(out gimic) && gimicSet != null)
            {
                gimic.SetGimicID(gimicSet.ID);
                gimicManager.AddGimic(gimic);
            }
        }

        BoxCollider boxCollider;
        if (tile.TryGetComponent(out boxCollider))
        {
            Floor tileFloor = tile.AddComponent<Floor>();
            
            tileFloor.floor = spawnFloor;
            // tileFloor.tilePos = tileSet.tilePos;
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
    
    Vector3 GetUnitTilePos(TileID tileID, Vector3 tilePos)
    {
        float floorUnit = prefabData.GetUnit(tileID);
        return tilePos + new Vector3(0, floorUnit, 0);
    }

    GimicSet GetGimicSet(List<GimicSet> gimicSetList, Vector3 spawnPos)
    {
        GimicSet gimicSet = gimicSetList.Find(x => Vector3.Equals(x.targetPos, spawnPos));
        if (gimicSet != null)
        {
            return gimicSet;
        }
        
        return null;
    }
    
    bool FindTopTile(Vector3 tilePos)
    {
        var data = spawnList.Find(x =>
            x.Item1 == tilePos + (Vector3.up * 7.5f) || 
            x.Item1 == tilePos + (Vector3.up * 10));

        return (data != null);
    }
}
