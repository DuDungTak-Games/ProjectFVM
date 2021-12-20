using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoSingleton<GameManager>
{

    public VMCamera vmCamera{ get; private set; }
    public VMInputManager inputManager{ get; private set; }
    public TileManager tileManager { get; private set; }
    public GimicManager gimicManager { get; private set; }
    public CanvasManager canvasManager { get; private set; }
    public Player player { get; private set; }

    GameState gameState;

    int vmKg;

    Dictionary<GameState, UnityEvent> gameEvents = new Dictionary<GameState, UnityEvent>();
    Dictionary<ItemType, int> itemInventory = new Dictionary<ItemType, int>();

    protected override void Init()
    {
        vmCamera = FindObjectOfType<VMCamera>();
        inputManager = FindObjectOfType<VMInputManager>();
        tileManager = FindObjectOfType<TileManager>();
        gimicManager = FindObjectOfType<GimicManager>();
        canvasManager = FindObjectOfType<CanvasManager>();
        player = FindObjectOfType<Player>();

        AddGameEvent(GameState.LOADING, () =>
        {
            canvasManager.SetPanel("Loading");
        });

        AddGameEvent(GameState.LOBBY, () =>
        {
            canvasManager.SetPanel("Lobby");
        });

        AddGameEvent(GameState.COIN_COLLECTION, () =>
        {
            inputManager.SetControlLock(false);

            canvasManager.SetPanel("InGame_Coin");
        });

        AddGameEvent(GameState.LAUNCH_PREPARE, () =>
        {
            inputManager.SetControlLock(true);

            canvasManager.SetPanel("Launch_Prepare");
        });

        AddGameEvent(GameState.LAUNCH_READY, () =>
        {
            canvasManager.SetPanel("Launch_Ready");
        });

        AddGameEvent(GameState.LAUNCH_PLAYING, () =>
        {
            canvasManager.SetPanel("Launch_Playing");
        });

        AddGameEvent(GameState.END, () =>
        {
            canvasManager.SetPanel("End");
        });

        SetGameState(GameState.LOADING);

        // TODO : Loading Coroutine
        // TileManager, Player Init 처리 필요

        SetGameState(GameState.COIN_COLLECTION);
    }

    public bool IsGameState(GameState state)
    {
        return (gameState == state);
    }

    public GameState GetGameState()
    {
        return gameState;
    }

    public void SetGameState(GameState type, bool isAction = true)
    {
        gameState = type;

        if (isAction)
        {
            ActionGameEvent(gameState);
        }
    }


    public void AddGameEvent(GameState type, UnityAction action)
    {
        if (gameEvents.ContainsKey(type))
        {
            gameEvents[type].AddListener(action);
        }
        else
        {
            UnityEvent unityEvent = new UnityEvent();
            unityEvent.AddListener(action);

            gameEvents.Add(type, unityEvent);
        }
    }

    public void ClearGameEvent(GameState type)
    {
        if (gameEvents.ContainsKey(type))
        {
            gameEvents[type].RemoveAllListeners();
        }
    }

    public void ActionGameEvent(GameState type)
    {
        if (gameEvents.ContainsKey(gameState))
        {
            gameEvents[gameState].Invoke();
        }
    }


    public void AddItem(ItemType type, int addValue = 1)
    {
        if (!itemInventory.ContainsKey(type))
        {
            itemInventory.Add(type, 0);
        }

        itemInventory[type] += addValue;

        UpdateItem(type);
    }

    public bool ConsumeItem(ItemType type, int consumeValue = 1)
    {
        if (itemInventory.ContainsKey(type))
        {
            if(itemInventory[type] >= consumeValue)
            {
                itemInventory[type] -= consumeValue;

                UpdateItem(type);

                return true;
            }
        }

        return false;
    }

    void UpdateItem(ItemType type)
    {
        if (itemInventory[type] < 0)
        {
            itemInventory[type] = 0;
        }

        ItemLabel.refresh.Invoke();
    }

    public void ClearItemInventory()
    {
        itemInventory.Clear();
    }

    public int GetItemCount(ItemType type)
    {
        if (itemInventory.ContainsKey(type))
        {
            return itemInventory[type];
        }

        return 0;
    }


    public void SetKg(int kg)
    {
        vmKg = kg;
    }

    public bool ConsumeKg(int consumeKg)
    {
        if (vmKg >= consumeKg)
        {
            vmKg -= consumeKg;

            if(vmKg < 0)
            {
                vmKg = 0;
            }

            ItemLabel.refresh.Invoke();

            return true;
        }

        return false;
    }

    public int GetKg()
    {
        return vmKg;
    }
}
