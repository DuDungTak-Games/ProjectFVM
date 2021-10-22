using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditorTile : MonoBehaviour
{
    [HideInInspector] 
    public TileID tileID;
    
    [HideInInspector]
    public float floor;
    
    [HideInInspector]
    public Vector3 spawnPos, spawnRot;
}
