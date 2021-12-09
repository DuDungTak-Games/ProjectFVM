using System;
using UnityEngine;

[Serializable]
public class GimicSet
{
    public GimicSet(GimicSet gimicSet = null)
    {
        if (gimicSet != null)
        {
            this.targetPos = gimicSet.targetPos;
            this.ID = gimicSet.ID;
        }
    }

    public Vector3 targetPos; // NOTE : TilePos
    public int ID;
}