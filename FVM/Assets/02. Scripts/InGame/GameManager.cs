using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoSingleton<GameManager>
{

    public VMCamera vmCamera{ get; private set; }
    public TileManager tileManager { get; private set; }
    public Player player { get; private set; }
    
    protected override void Init()
    {
        vmCamera = FindObjectOfType<VMCamera>();
        tileManager = FindObjectOfType<TileManager>();
        player = FindObjectOfType<Player>();
    }
    
    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
