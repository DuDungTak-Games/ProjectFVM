using UnityEngine;

public class Panel : MonoBehaviour
{

    public TestGameManager.eventType eventType = TestGameManager.eventType.PREPARE;

    void Awake()
    {
        TestGameManager.Instance.AddGameEvent(eventType, OnPanel);
        
        this.gameObject.SetActive(false);
    }

    void OnPanel()
    {
        this.gameObject.SetActive(true);
    }
}
