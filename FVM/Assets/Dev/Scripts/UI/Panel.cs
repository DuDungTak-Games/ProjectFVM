using UnityEngine;

public class Panel : MonoBehaviour
{

    public gameState onEventType = gameState.Prepare;

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
        TestGameManager.Instance.SetGameEvent(gameState.CoinGame, false);
        
        this.gameObject.SetActive(false);
    }
}
