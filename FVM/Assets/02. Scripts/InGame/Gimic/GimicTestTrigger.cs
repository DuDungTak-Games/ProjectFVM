using System;
using UnityEngine;

public class GimicTestTrigger : GimicTrigger
{
 
    TileSkin[] tileSkins;
    
    void Start()
    {
        tileSkins = GetComponentsInChildren<TileSkin>();

        foreach (var skin in tileSkins)
        {
            skin.Init(ID);
        }
    }
}
