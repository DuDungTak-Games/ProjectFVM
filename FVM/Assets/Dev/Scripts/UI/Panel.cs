using UnityEngine;

public class Panel : MonoBehaviour
{

    public GameState onEventType = GameState.PREPARE;

    void Awake()
    {
        TestGameManager.Instance.AddGameEvent(onEventType, OnPanel);
        
        this.gameObject.SetActive(false);
    }

    void OnPanel()
    {
        this.gameObject.SetActive(true);
    }
    
    public void Close()
    {
        TestGameManager.Instance.SetGameEvent(GameState.COIN_GAME);
        
        this.gameObject.SetActive(false);
    }
}
