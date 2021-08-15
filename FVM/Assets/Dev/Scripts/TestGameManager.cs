using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Events;

using labelType = LabelText.labelType;

public class TestGameManager : MonoBehaviour
{

    public static TestGameManager Instance;

    private int coin = 0;

    Dictionary<labelType, UnityEvent<int>> updateTextEvents = new Dictionary<labelType, UnityEvent<int>>();

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
        UnityEvent<int> unityEvent = new UnityEvent<int>(); 
        unityEvent.AddListener(action);
        updateTextEvents.Add(type, unityEvent);
    }
    
    public void GetCoinEvent(int increase = 1)
    {
        coin += increase;

        UpdateUI(labelType.COIN, coin);
    }

    private void UpdateUI(labelType type, int value)
    {
        updateTextEvents[type].Invoke(value);
    }
}
