using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoSingleton<GameManager>
{

    public VMCamera vmCamera{ get; private set; }
    public TileManager tileManager { get; private set; }
    public Player player { get; private set; }
    
    void Awake()
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
