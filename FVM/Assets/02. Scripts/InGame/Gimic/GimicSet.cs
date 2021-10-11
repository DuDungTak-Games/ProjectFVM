using System;
using UnityEngine;

[Serializable]
public struct GimicSet
{
    public GimicSet(Vector3 targetPos, int ID)
    {
        this.targetPos = targetPos;
        this.ID = ID;
    }
    
    public bool Check(Vector3 comparePos)
    {
        return Vector3.Equals(targetPos, comparePos);
    }

    public Vector3 targetPos;
    public int ID;
}