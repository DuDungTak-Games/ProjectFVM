using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

using labelType = LabelText.labelType;

public class TestGameManager : MonoBehaviour
{

    public static TestGameManager Instance;

    public enum eventType
    {
        PREPARE,
        GAMEOVER
    }

    private int coin = 0, kg = 120, resultHeight;

    Dictionary<labelType, UnityEvent<int>> updateTextEvents = new Dictionary<labelType, UnityEvent<int>>();
    Dictionary<eventType, UnityEvent> gameEvents = new Dictionary<eventType, UnityEvent>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else if(Instance != this)
        {
            Destroy(this.gameObject);
        }
    }

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

    public void AddGameEvent(eventType type, UnityAction action)
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

    public void OnGameEvent(eventType type)
    {
        gameEvents[type].Invoke();
    }
    
    public void UpdateUI(labelType type, int value)
    {
        updateTextEvents[type].Invoke(value);
    }
}
