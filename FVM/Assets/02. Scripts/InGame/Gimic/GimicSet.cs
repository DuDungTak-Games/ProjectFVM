using System;
using UnityEngine;

[Serializable]
public struct GimicSet
{
    public GimicSet(Vector3 targetPos, int gimicID)
    {
        this.targetPos = targetPos;
        this.gimicID = gimicID;
    }

    public Vector3 targetPos;
    public int gimicID;
}