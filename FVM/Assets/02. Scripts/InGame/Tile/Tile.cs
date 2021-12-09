using UnityEngine;

public class Tile : MonoBehaviour
{
    public TileID tileID;
    
    public float floor;

    public bool IsGimic()
    {
        return (tileID >= TileID.GIMIC_CUSTOM);
    }

    // NOTE : �Ϲ� Ÿ�ϸ� halfHeightUnit ���
    public Vector3 GetDirPos(Vector3 direction)
    {
        float unit = (floor % 1 != 0) && !IsGimic() ? TileManager.halfHeightUnit : TileManager.tileUnit;
        return transform.position + (direction * unit);
    }
}
