using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestTileManager : MonoBehaviour
{

    public GameObject testTile_Prefab;
    public GameObject testCoin_Prefab;

    float hUnit = 10f, vUnit = 5f;

    Transform rootTrf;

    List<TileSet> tileList = new List<TileSet>();
    List<TileSet> coinList = new List<TileSet>();

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

        SpawnTile();
        SpawnCoin();
    }

    void InitTile()
    {
        AddTileSet(0, 0, 1);

        for (int repeat = -5; repeat < 5; repeat++)
        {
            if (repeat == 0)
                continue;

            for (int x = -5; x < 5; x++)
            {
                if (x == 0)
                    continue;
                AddTileSet(x, repeat, 1);
            }

            for (int z = -5; z < 5; z++)
            {
                if (z == 0)
                    continue;
                AddTileSet(0, repeat, 1);
            }
        }

        for (int x = -5; x < 5; x++)
        {
            AddTileSet(x, 6, 3f);
        }

        for (int x = -5; x < 5; x++)
        {
            AddTileSet(x, 6, 3.5f);
        }

        for (int x = -5; x < 5; x++)
        {
            AddTileSet(x, 7, 4f);
        }

        for (int x = -5; x < 5; x++)
        {
            AddTileSet(x, 5, 1);
        }

        for (int x = -5; x < 5; x++)
        {
            AddTileSet(x, 5, 2);
        }

        for (int x = -5; x < 5; x++)
        {
            AddTileSet(x, 5, 3);
        }

        for (int x = -5; x < 5; x++)
        {
            AddTileSet(x, 4, 2.5f);
        }

        for (int x = -5; x < 5; x++)
        {
            AddTileSet(x, 4, 2f);
        }

        for (int x = -5; x < 5; x++)
        {
            AddTileSet(x, 3, 2f);
        }

        for (int x = -5; x < 5; x++)
        {
            AddTileSet(x, 2, 1.5f);
        }
    }

    void InitCoin()
    {
        AddCoinSet(0, 1, 1);
        AddCoinSet(0, 2, 1.5f);
        AddCoinSet(0, 3, 2);
        AddCoinSet(0, 4, 2.5f);
        AddCoinSet(0, 5, 3);
    }

    void SpawnTile()
    {
        foreach(TileSet tileSet in tileList)
        {
            GameObject tile = Instantiate(testTile_Prefab, tileSet.spawnPos, Quaternion.identity, rootTrf);
            
            if(tile.TryGetComponent<Floor>(out Floor tileFloor))
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

    void AddTileSet(float x, float z, float spawnFloor)
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

        tileList.Add(CreateTileSet(pos, spawnFloor));
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

    TileSet CreateTileSet(Vector3 pos, float floor)
    {
        TileSet tileSet = new TileSet();
        tileSet.SetData(pos, floor);
        return tileSet;
    }
}
