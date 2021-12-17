using UnityEngine;

public class GameButton : MonoBehaviour
{

    [SerializeField] GameState targetState;

    public void ChangeGameState()
    {
        GameManager.Instance.SetGameState(targetState);
    }
}
