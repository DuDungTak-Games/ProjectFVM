using System;
using UnityEngine;

[Serializable]
public class TileSet
{
    public TileSet(TileSet tileSet = null)
    {
        if (tileSet != null)
        {
            this.spawnPos = tileSet.spawnPos;
            this.spawnRot = tileSet.spawnRot;
            this.spawnFloor = tileSet.spawnFloor;
        }
    }

    public Vector3 spawnPos; // 보정된 타일 스폰 위치
    public Vector3 spawnRot;
    public float spawnFloor; // 타일 위치 층
}
