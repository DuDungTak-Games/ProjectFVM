using System;
using UnityEngine;

[Serializable]
public struct TileSet
{
    public TileSet(Vector2 tilePos, Vector3 spawnPos, Vector3 spawnRot, float spawnFloor)
    {
        this.tilePos = tilePos;
        this.spawnPos = spawnPos;
        this.spawnRot = spawnRot;
        this.spawnFloor = spawnFloor;
    }

    public bool Check(TileSet tileSet, bool compare)
    {
        return Equals(tilePos, tileSet.tilePos) && compare;
    }

    public Vector2 tilePos;
    public Vector3 spawnPos;
    public Vector3 spawnRot;
    public float spawnFloor;
}
