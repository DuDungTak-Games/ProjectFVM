using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

using labelType = LabelText.labelType;

public class TestGameManager : MonoSingleton<TestGameManager>
{

    private GameState gameState;

    private int coin = 0, kg = 120, resultHeight;

    Dictionary<labelType, UnityEvent<int>> updateTextEvents = new Dictionary<labelType, UnityEvent<int>>();
    Dictionary<GameState, UnityEvent> gameEvents = new Dictionary<GameState, UnityEvent>();

    public void AddUpdateTextEvent(labelType type, UnityAction<int> action)
    {
        if (!updateTextEvents.ContainsKey(type))
        {
            UnityEvent<int> unityEvent = new UnityEvent<int>(); 
            unityEvent.AddListener(action);
            
            updateTextEvents.Add(type, unityEvent);
        }
        else
        {
            updateTextEvents[type].AddListener(action);
        }
    }

    public void AddGameEvent(GameState type, UnityAction action)
    {
        if (!gameEvents.ContainsKey(type))
        {
            UnityEvent unityEvent = new UnityEvent(); 
            unityEvent.AddListener(action);
            
            gameEvents.Add(type, unityEvent);
        }
        else
        {
            gameEvents[type].AddListener(action);
        }
    }

    public void GetCoinEvent(int increase = 1)
    {
        coin += increase;

        UpdateUI(labelType.COIN, coin);
    }

    public void DietVM(int kg)
    {
        this.kg -= kg;
    }

    public void BuyProductEvent(int count)
    {
        coin -= count;
        
        UpdateUI(labelType.COIN, coin);
    }

    public int GetKg()
    {
        return kg;
    }

    public int GetCoinCount()
    {
        return coin;
    }

    public void SetGameEvent(GameState type, bool isUpdate = true)
    {
        gameState = type;

        if (!isUpdate)
            return;
        
        if (gameEvents.ContainsKey(gameState))
        {
            gameEvents[gameState].Invoke();
        }
    }

    public bool IsGameState(GameState type)
    {
        return gameState == type;
    }
    
    public void UpdateUI(labelType type, int value)
    {
        updateTextEvents[type].Invoke(value);
    }
}
