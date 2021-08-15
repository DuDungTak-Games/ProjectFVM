using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DuDungTakGames.Gimic;

public class TestTileManager : MonoBehaviour
{

    public GameObject testTile_Prefab;
    public GameObject testCoin_Prefab;
    public GameObject testTrigger_Prefab;
    public GameObject testVM_Prefab;

    float hUnit = 10f, vUnit = 5f;

    Transform rootTrf;

    List<TileSet> tileList = new List<TileSet>();
    List<TileSet> coinList = new List<TileSet>();
    List<TileSet> triggerList = new List<TileSet>();
    List<TileSet> gimicList = new List<TileSet>();

    // NOTE : TEST GAMEOBJECT LIST!!!
    List<GameObject> gimicObjList = new List<GameObject>();

    const float START_FLOOR_UNIT = -5f;
    const float HALF_FLOOR_UNIT = 2.5f;
    const float START_COIN_FLOOR_UNIT = 5f;

    struct TileSet
    {
        public void SetData(Vector3 spawnPos, float spawnFloor)
        {
            this.spawnPos = spawnPos;
            this.spawnFloor = spawnFloor;
        }

        public Vector3 spawnPos { get; private set; }
        public float spawnFloor { get; private set; }
    }

    void Start()
    {
        Init();
    }

    void Init()
    {
        GameObject root = new GameObject();
        rootTrf = root.transform;
        rootTrf.name = "TileList";

        InitTile();
        InitCoin();
        InitGimic();
        InitTrigger();

        SpawnTile();
        SpawnCoin();
        SpawnGimic();
        SpawnTrigger();

        // TODO : �÷��̾� ���� ��ġ ���� �߰�
        //SpawnStartPoint();
        SpawnGamePoint(2, 3, 0);
    }

    void InitTile()
    {
        AddTileSet(0, 0, 1);

        AddTileSet(1, 0, 0.5f);
        AddTileSet(-1, 2, 1.5f);
        AddTileSet(-3, 2, 2.5f);

        AddTileSet(-3, 2, 2);
        AddTileSet(-3, 1, 2);
        AddTileSet(-3, 1, 3);

        for (int x = -3; x < 1; x++)
        {
            for (int z = 1; z < 6; z++)
            {
                for (int y = -2; y < 2; y++)
                {
                    AddTileSet(x, z, y);
                }
            }
        }

        for (int x = 0; x < 4; x++)
        {
            for (int y = -2; y < 1; y++)
            {
                AddTileSet(x, 0, y);
            }
        }

        for (int x = 1; x < 4; x++)
        {
            for (int z = 1; z < 6; z++)
            {
                for (int y = -2; y < 1; y++)
                {
                    AddTileSet(x, z, y);
                }
            }
        }

        for (int x = -3; x < 2; x++)
        {
            for (int z = 3; z < 6; z++)
            {
                AddTileSet(x, z, 2);
            }
        }

        for (int z = 1; z < 6; z++)
        {
            AddTileSet(1, z, 1);
        }

        for (int x = 2; x < 4; x++)
        {
            for (int y = 1; y < 3; y++)
            {
                AddTileSet(x, 5, y);
            }
        }
    }

    void InitCoin()
    {
        AddCoinSet(-3, 1, 3);
        AddCoinSet(-2, 2, 1);
        AddCoinSet(-2, 3, 2);
        AddCoinSet(-1, 5, 2);
        AddCoinSet(3, 5, 2);

        AddCoinSet(2, 1, 0);
        AddCoinSet(2, 2, 0);
    }

    void InitTrigger()
    {
        AddTriggerSet(3, 0, 0);
        AddTriggerSet(-3, 5, 2);
    }

    void InitGimic()
    {
        AddGimicSet(-1, 3, 3);
    }

    void SpawnTile()
    {
        foreach(TileSet tileSet in tileList)
        {
            GameObject tile = Instantiate(testTile_Prefab, tileSet.spawnPos, Quaternion.identity, rootTrf);
            
            if(tile.TryGetComponent(out Floor tileFloor))
            {
                float spawnFloor = tileSet.spawnFloor;
                tileFloor.floor = spawnFloor;

                if (spawnFloor % 1f == 0.5f)
                {
                    Vector3 scale = tile.transform.localScale;
                    scale.y *= 0.5f;
                    tile.transform.localScale = scale;
                }
            }
        }
    }

    void SpawnCoin()
    {
        foreach (TileSet tileSet in coinList)
        {
            GameObject coin = Instantiate(testCoin_Prefab, tileSet.spawnPos, testCoin_Prefab.transform.rotation, rootTrf);
        }
    }

    void SpawnTrigger()
    {
        foreach (TileSet tileSet in triggerList)
        {
            GameObject trigger = Instantiate(testTrigger_Prefab, tileSet.spawnPos, testTrigger_Prefab.transform.rotation, rootTrf);

            if (trigger.TryGetComponent(out Gimic tileTrigger))
            {
                tileTrigger.SetTriggerAction(() => { gimicObjList[0].SetActive(false); });
            }
        }
    }

    void SpawnGimic()
    {
        foreach (TileSet tileSet in gimicList)
        {
            GameObject tile = Instantiate(testTile_Prefab, tileSet.spawnPos, Quaternion.identity, rootTrf);

            if (tile.TryGetComponent(out Floor tileFloor))
            {
                float spawnFloor = tileSet.spawnFloor;
                tileFloor.floor = spawnFloor;

                if (spawnFloor % 1f == 0.5f)
                {
                    Vector3 scale = tile.transform.localScale;
                    scale.y *= 0.5f;
                    tile.transform.localScale = scale;
                }
            }

            if (tile.TryGetComponent(out MeshRenderer mr))
            {
                mr.material.color = Color.red;
            }

            gimicObjList.Add(tile);
        }
    }

    void SpawnGamePoint(float x, float z, float spawnFloor)
    {
        float posX = x * hUnit;
        float posY = 0;
        float posZ = z * hUnit;

        posY += (Mathf.FloorToInt(spawnFloor) * vUnit) + ((Mathf.FloorToInt(spawnFloor) - 1) * vUnit);
        posY += spawnFloor % 1 == 0 ? -vUnit : 0;

        Vector3 pos = new Vector3(posX, posY, posZ);



        GameObject vendingMachine = Instantiate(testVM_Prefab, new Vector3(posX, posY, (z+1) * hUnit), Quaternion.Euler(0, 180, 0));
        GameObject trigger = Instantiate(testTrigger_Prefab, pos, testTrigger_Prefab.transform.rotation, rootTrf);

        if (trigger.TryGetComponent(out Gimic tileTrigger))
        {
            tileTrigger.SetTriggerAction(() => { UnityEngine.SceneManagement.SceneManager.LoadScene(1); });
        }
    }

    void AddTileSet(float x, float z, float spawnFloor)
    {
        float posX = x * hUnit;
        float posY = START_FLOOR_UNIT;
        float posZ = z * hUnit;

        if (spawnFloor != 1f)
        {
            posY += (Mathf.FloorToInt(spawnFloor) * vUnit) + ((Mathf.FloorToInt(spawnFloor) - 1) * vUnit);
            posY += spawnFloor % 1 == 0 ? -vUnit : HALF_FLOOR_UNIT;
        }

        Vector3 pos = new Vector3(posX, posY, posZ);

        // TODO : pos �����θ� �ߺ� üũ�� �� �� �ֵ��� ���� �ʿ�
        TileSet tileSet = CreateTileSet(pos, spawnFloor);
        if(!tileList.Contains(tileSet))
        {
            tileList.Add(tileSet);
        }
        else
        {
            Debug.LogWarningFormat("[�ߺ� Ÿ��] TileSet : {0} / {1}", tileSet.spawnPos, tileSet.spawnFloor);
        }
    }

    void AddCoinSet(float x, float z, float spawnFloor)
    {
        float posX = x * hUnit;
        float posY = START_COIN_FLOOR_UNIT;
        float posZ = z * hUnit;

        posY += (Mathf.FloorToInt(spawnFloor) * vUnit) + ((Mathf.FloorToInt(spawnFloor) - 1) * vUnit);
        posY += spawnFloor % 1 == 0 ? -vUnit : 0;

        Vector3 pos = new Vector3(posX, posY, posZ);

        coinList.Add(CreateTileSet(pos, spawnFloor));
    }

    void AddTriggerSet(float x, float z, float spawnFloor)
    {
        float posX = x * hUnit;
        float posY = 0;
        float posZ = z * hUnit;

        posY += (Mathf.FloorToInt(spawnFloor) * vUnit) + ((Mathf.FloorToInt(spawnFloor) - 1) * vUnit);
        posY += spawnFloor % 1 == 0 ? -vUnit : 0;

        Vector3 pos = new Vector3(posX, posY, posZ);

        triggerList.Add(CreateTileSet(pos, spawnFloor));
    }

    void AddGimicSet(float x, float z, float spawnFloor)
    {
        float posX = x * hUnit;
        float posY = START_FLOOR_UNIT;
        float posZ = z * hUnit;

        if (spawnFloor > 1f)
        {
            posY += (Mathf.FloorToInt(spawnFloor) * vUnit) + ((Mathf.FloorToInt(spawnFloor) - 1) * vUnit);
            posY += spawnFloor % 1 == 0 ? -vUnit : HALF_FLOOR_UNIT;
        }

        Vector3 pos = new Vector3(posX, posY, posZ);

        gimicList.Add(CreateTileSet(pos, spawnFloor));
    }

    TileSet CreateTileSet(Vector3 pos, float floor)
    {
        TileSet tileSet = new TileSet();
        tileSet.SetData(pos, floor);
        return tileSet;
    }
}
