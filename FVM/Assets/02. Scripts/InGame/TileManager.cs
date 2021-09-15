using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using DuDungTakGames.Extensions;
using DuDungTakGames.Gimic;

public class TileManager : MonoBehaviour
{
    
    // NOTE : 개발 테스트용
    public GameObject tile_Prefab;
    public GameObject coin_Prefab;
    public GameObject startPoint_Prefab;
    public GameObject vmPoint_Prefab;
    public GameObject trigger_Prefab;
    public GameObject gimic_Prefab;
    public Material subTileMaterial;
    
    
    
    Dictionary<TileID, GameObject> prefabList = new Dictionary<TileID, GameObject>(); // 타일 프리팹 리스트
    Dictionary<TileID, float> floorUnitList = new Dictionary<TileID, float>(); // 타일 스폰 Floor 단위 리스트
    Dictionary<TileID, List<TileSet>> tileSetList = new Dictionary<TileID, List<TileSet>>(); // 타일 배치 정보 리스트
    Dictionary<TileID, List<GameObject>> spawnList = new Dictionary<TileID, List<GameObject>>(); // 스폰된 타일 리스트

    Transform rootTrf;
    
    TileID curTileID;

    // NOTE : 수직축, 수평축 위치 보정
    const float H_UNIT = 10;
    const float V_UNIT = 10;
    
    // NOTE : Floor 보정
    const float START_FLOOR_UNIT = 0;
    const float HALF_FLOOR_UNIT = 7.5f;
    const float START_COIN_FLOOR_UNIT = 5f;
    
    void Awake()
    {
        Init();
    }
    
    void Start()
    {
        
    }

    void Update()
    {
        
    }

    void Init()
    {
        GameObject tileFolder = new GameObject("Tile Folder");
        rootTrf = tileFolder.transform;

        SetPrefab();
        SetFloorUnit();
        
        SetPoint();
        SetTile();
        SetCoin();
        SetGimic();

        foreach (TileID tileID in tileSetList.Keys)
        {
            spawnList.Add(tileID, new List<GameObject>());
            
            if(tileID == TileID.HIDDEN_TILE)
                continue;
            
            SpawnTile(tileID);
        }
        
        spawnList[TileID.TILE].Clear();
    }

    
    
    // NOTE : 개발 테스트용 세팅
    void SetPrefab()
    {
        prefabList.Add(TileID.TILE, tile_Prefab);
        prefabList.Add(TileID.HIDDEN_TILE, tile_Prefab);
        prefabList.Add(TileID.COIN, coin_Prefab);
        prefabList.Add(TileID.START_POINT, startPoint_Prefab);
        prefabList.Add(TileID.VM_POINT, vmPoint_Prefab);
        prefabList.Add(TileID.FOOTHOLD_TRIGGER, trigger_Prefab);
        prefabList.Add(TileID.BOX, gimic_Prefab);
    }
    
    void SetFloorUnit()
    {
        floorUnitList.Add(TileID.COIN, 12);
        floorUnitList.Add(TileID.START_POINT, 5);
        floorUnitList.Add(TileID.VM_POINT, 5);
        floorUnitList.Add(TileID.FOOTHOLD_TRIGGER, 5.5f);
    }
    
    void SetTile()
    {
        curTileID = TileID.TILE;

        AddTileSet(0, 0, 0);
        AddTileSet(0, 1, 0);
        AddTileSet(0, 1, -1);
        for (int y = -2; y <= 0; y++) { AddTileSet(0, 2, y); }
        for (int z = 3; z <= 10; z++)
        {
            for (int y = -4; y <= 0; y++)
            {
                AddTileSet(0, z, y);
            }
        }
        for (int x = 1; x <= 4; x++) { AddTileSet(x, 0, 0); }
        for (int x = 2; x <= 4; x++) { AddTileSet(x, 0, -1); }
        AddTileSet(2, 0, -1);
        for (int x = 3; x <= 4; x++) { AddTileSet(x, 0, -2); }
        for (int x = 3; x <= 4; x++) { AddTileSet(x, 0, -3); }
        AddTileSet(4, 0, -4);
        for (int x = -4; x <= -1; x++)
        {
            for (int y = -4; y <= 0; y++) { AddTileSet(x, 0, y); }
        }
        
        // 올라가는 루트
        for (int x = -4; x <= -1; x++) { AddTileSet(x, 1, 0); }
        AddTileSet(-4, 2, 0.5f);
        AddTileSet(-4, 3, 1);
        AddTileSet(-4, 4, 1.5f);
        AddTileSet(-4, 4, 1);
        AddTileSet(-4, 5, 2);
        for (int z = 2; z <= 4; z++) { AddTileSet(-3, z, 0); }
        for (int z = 2; z <= 4; z++) { AddTileSet(-2, z, 0); }
        for (int z = 2; z <= 4; z++) { AddTileSet(-1, z, 0); }
        for (int y = 1; y <= 3; y++) { AddTileSet(-3, 5, y); }
        for (int y = 1; y <= 3; y++) { AddTileSet(-2, 5, y); }
        for (int y = 1; y <= 3; y++) { AddTileSet(-1, 5, y); }
        for (int x = -3; x <= -1; x++) { AddTileSet(x, 6, 3); }
        for (int x = -3; x <= -1; x++) { AddTileSet(x, 7, 3); }
        for (int y = 1; y <= 3; y++) { AddTileSet(-1, 6, y); }
        for (int y = 1; y <= 3; y++) { AddTileSet(-1, 7, y); }
        AddTileSet(-4, 6, 2.5f);
        AddTileSet(-4, 7, 3);
        
        // 자판기로 가는 루트
        AddTileSet(0, -1, -0.5f);
        for (int y = -4; y <= -1; y++) { AddTileSet(0, -1, y); }
        AddTileSet(1, -1, -1);
        for (int y = -3; y <= -2; y++) { AddTileSet(1, -1, y); }
        AddTileSet(2, -1, -1.5f);
        for (int y = -3; y <= -2; y++) { AddTileSet(2, -1, y); }
        AddTileSet(2, -2, -2);
        AddTileSet(2, -3, -2.5f);
        for (int z = -3; z <= -2; z++) { AddTileSet(2, z, -3); }
        AddTileSet(2, -4, -3);
        AddTileSet(3, -4, -3.5f);
        for (int z = -3; z <= -1; z++) { AddTileSet(3, z, -4); }
        for (int x = 2; x <= 3; x++) { AddTileSet(x, -4, -4); }
        
        
        
        curTileID = TileID.HIDDEN_TILE;
        
        AddTileSet(4, 1, 0);
        AddTileSet(4, 2, 0);
        AddTileSet(4, 3, 0);
        AddTileSet(4, 4, 0);
    }
    
    void SetPoint()
    {
        curTileID = TileID.START_POINT;
        
        AddTileSet(-1, 2, 0);
        
        
        
        curTileID = TileID.VM_POINT;
        
        AddTileSet(3, -1, -4);
    }

    void SetCoin()
    {
        curTileID = TileID.COIN;

        for (int z = 6; z <= 10; z++) { AddTileSet(0, z, 0); }
        for (int x = -3; x <= -1; x++) { AddTileSet(x, 7, 3); }
        for (int x = -3; x <= -1; x++) { AddTileSet(x, 6, 3); }
        for (int x = -3; x <= -1; x++) { AddTileSet(x, 5, 3); }
    }

    void SetGimic()
    {
        curTileID = TileID.BOX;

        AddTileSet(0, 5, 1);
        
        
        
        curTileID = TileID.FOOTHOLD_TRIGGER;

        AddTileSet(4, 0, 0);
    }
    //
    
    
    
    void SpawnTile(TileID tileID)
    {
        foreach (TileSet tileSet in tileSetList[tileID])
        {
            GameObject tileObj = Instantiate(prefabList[tileID], tileSet.spawnPos, prefabList[tileID].transform.rotation, rootTrf);
            spawnList[tileID].Add(tileObj);

            float spawnFloor = tileSet.spawnFloor;
            switch (tileID)
            {
                case TileID.COIN:
                    break;
                case TileID.START_POINT:
                    Transform playerTrf = FindObjectOfType<Player>().transform;
                    playerTrf.SetPosition(tileSet.spawnPos);
                    break;
                case TileID.VM_POINT:
                    // TODO : 별도로 분리 필요
                    Vector3 triggerPos = tileSet.spawnPos + (tileObj.transform.forward * H_UNIT);
                    GameObject trigger = Instantiate(prefabList[TileID.FOOTHOLD_TRIGGER], triggerPos, prefabList[TileID.FOOTHOLD_TRIGGER].transform.rotation, rootTrf);

                    if (trigger.TryGetComponent(out Gimic tileTrigger))
                    {
                        tileTrigger.SetTriggerAction(() => { TestGameManager.Instance.SetGameEvent(GameState.PREPARE); });
                    }
                    break;
                case TileID.FOOTHOLD_TRIGGER:
                    if (tileObj.TryGetComponent(out Gimic _tileTrigger))
                    {
                        _tileTrigger.SetTriggerAction(() =>
                        {
                            SpawnTile(TileID.HIDDEN_TILE);
                            spawnList[TileID.BOX][0].SetActive(false);
                        });
                    }
                    break;
                case TileID.BOX:
                    if (tileObj.TryGetComponent(out MeshRenderer _mr))
                    {
                        _mr.material.color = Color.red;
                    };
                    
                    tileObj.AddComponent<BoxCollider>();
                    break;
                default:
                    if (spawnFloor % 1f != 0)
                    {
                        tileObj?.transform.MultipleScaleY(0.5f);
                    }
                
                    bool hasTop = false; 
                    tileSetList[tileID].Find(x => hasTop = 
                        x.Check(tileSet, Equals(x.spawnFloor, spawnFloor+1) || Equals(x.spawnFloor, spawnFloor+0.5f)));

                    if (hasTop)
                    {
                        if (tileObj.TryGetComponent(out MeshRenderer mr))
                        {
                            mr.material = subTileMaterial;
                        };
                    }
                    else
                    {
                        tileObj.AddComponent<BoxCollider>();
                        tileObj.AddComponent<Tile>();
                    }
                    break;
            }
            
            if (tileObj.TryGetComponent(out Floor tileFloor))
            {
                tileFloor.floor = spawnFloor;
                tileFloor.tilePos = tileSet.tilePos; // DEBUG
            }
        }
        
        tileSetList[tileID].Clear();
    }

    void AddTileSet(int x, int z, float floor)
    {
        if (!tileSetList.ContainsKey(curTileID))
        {
            tileSetList.Add(curTileID, new List<TileSet>());
        }
        
        float unit = floorUnitList.ContainsKey(curTileID) ? floorUnitList[curTileID] : 0;
        float posX = (x * H_UNIT);
        float posY = unit + (Mathf.FloorToInt(floor) * V_UNIT);
        float posZ = (z * H_UNIT);

        // NOTE : 0.5층 단위 보정
        if (floor % 1 != 0)
        {
            posY += HALF_FLOOR_UNIT;
        }

        Vector2 tilePos = new Vector2(x, z); // DEBUG
        Vector3 spawnPos = new Vector3(posX, posY, posZ);

        TileSet tileSet = new TileSet(tilePos, spawnPos, floor);
        tileSetList[curTileID].Add(tileSet);
    }
}
