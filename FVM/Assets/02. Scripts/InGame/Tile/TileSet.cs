using UnityEngine;

public struct TileSet
{
    public TileSet(Vector2 tilePos, Vector3 spawnPos, float spawnFloor)
    {
        this.tilePos = tilePos;
        this.spawnPos = spawnPos;
        this.spawnFloor = spawnFloor;
    }

    public bool Check(TileSet tileSet, bool compare)
    {
        return Equals(tilePos, tileSet.tilePos) && compare;
    }

    public Vector2 tilePos { get; private set; }
    public Vector3 spawnPos { get; private set; }
    public float spawnFloor { get; private set; }
}
