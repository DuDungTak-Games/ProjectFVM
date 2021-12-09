using System;
using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{

    public PlayerController controller { get; private set; }

    void Awake()
    {
        controller = this.GetComponent<PlayerController>();

        Init();
    }
    
    void Init()
    {
        TestGameManager.Instance.SetGameEvent(GameState.COIN_GAME);
    }
}
