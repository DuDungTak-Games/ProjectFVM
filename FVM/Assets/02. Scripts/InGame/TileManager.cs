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

    public const float tileUnit = 10f;
    public const float halfHeightUnit = 7.5f;

    List<Tuple<Vector3, GameObject>> spawnList = new List<Tuple<Vector3, GameObject>>();
    
    GimicManager gimicManager;
    
    Transform rootTrf;

    void Awake()
    {
        Init();
    }

    void Init()
    {
        gimicManager = FindObjectOfType<GimicManager>();
        
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
        GameObject tileObj = Instantiate(prefab, GetPosByOffset(tileID, tileSet.spawnPos), 
                            Quaternion.Euler(tileSet.spawnRot), rootTrf);
        tileObj.name = tileObj.name.Replace("(Clone)", "").Trim();
        
        spawnList.Add(new Tuple<Vector3, GameObject>(tileSet.spawnPos, tileObj));

        float spawnFloor = tileSet.spawnFloor;
        switch (tileID)
        {
            case TileID.START_POINT:
                PlayerController pc = GameManager.Instance.player.controller;
                pc.transform.SetPosition(tileObj.transform.position);
                pc.transform.SetRotation(tileObj.transform.rotation);
                pc.SetFloor(spawnFloor, true);
                break;
            case TileID.VM_POINT:
                GameObject triggerPrefab = GetTilePrefab(TileID.PRESSURE_TOGGLE, tileSet);
                Vector3 triggerPos = GetPosByOffset(TileID.PRESSURE_TOGGLE, tileSet.spawnPos) + (tileObj.transform.forward.normalized * 10f);

                GameObject triggerObj = Instantiate(triggerPrefab, triggerPos, Quaternion.Euler(tileSet.spawnRot), rootTrf);
                GimicTrigger trigger = triggerObj.GetComponent<GimicTrigger>();
                trigger.SetGimicID(-1);
                trigger.AddEvent(() => { TestGameManager.Instance.SetGameEvent(GameState.PREPARE); });
                break;
            default:
                break;
        }
        
        if ((int) tileID >= (int) TileID.GIMIC_CUSTOM)
        {
            GimicObject gimic;
            if (tileObj.TryGetComponent(out gimic) && gimicSet != null)
            {
                gimic.SetGimicID(gimicSet.ID);
                gimicManager.AddGimic(gimic);
            }
        }

        BoxCollider boxCollider;
        if (tileObj.TryGetComponent(out boxCollider))
        {
            Tile tile = tileObj.AddComponent<Tile>();
            tile.tileID = tileID;
            tile.floor = spawnFloor;
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

    Vector3 GetPosByOffset(TileID tileID, Vector3 tilePos)
    {
        Vector3 offset = prefabData.GetOffset(tileID);
        return (tilePos + offset);
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
            x.Item1 == tilePos + (Vector3.up * halfHeightUnit) || 
            x.Item1 == tilePos + (Vector3.up * tileUnit));

        return (data != null);
    }
}
