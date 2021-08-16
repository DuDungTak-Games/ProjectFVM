using UnityEngine;

public class Panel : MonoBehaviour
{

    void Awake()
    {
        TestGameManager.Instance.AddPrepareGameEvent(TestGameManager.eventType.PREPARE, OnPanel);
        
        this.gameObject.SetActive(false);
    }

    void OnPanel()
    {
        this.gameObject.SetActive(true);
    }
}
