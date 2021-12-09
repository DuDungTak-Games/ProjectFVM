using UnityEngine;

public class Tile : MonoBehaviour
{
    public TileID tileID;
    
    public float floor;

    public bool IsGimic()
    {
        return (tileID >= TileID.GIMIC_CUSTOM);
    }

    // NOTE : 일반 타일만 halfHeightUnit 사용
    public Vector3 GetDirPos(Vector3 direction)
    {
        float unit = (floor % 1 != 0) && !IsGimic() ? TileManager.halfHeightUnit : TileManager.tileUnit;
        return transform.position + (direction * unit);
    }
}
