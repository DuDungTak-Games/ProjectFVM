using UnityEngine;
using UnityEngine.Events;

using DuDungTakGames.Input;

public class VMInputScreen : MonoBehaviour
{

    VMInputManager inputManager;

    public enum ScreenTouchType
    {
        NONE = 0, 
        RIGHT = 1, 
        LEFT = -1
    }
    
    [HideInInspector]
    public UnityEvent<ScreenTouchType> onTouch;

    void Awake()
    {
        inputManager = VMInputManager.Instance;

        Init();
    }

    void Init()
    {
        inputManager.inputEvents[TouchPhase.Stationary].AddListener(CheckScreenTouch);
    }

    void OnDestroy()
    {
        inputManager.inputEvents[TouchPhase.Stationary].RemoveListener(CheckScreenTouch);
    }

    void CheckScreenTouch(InputData inputData)
    {
        float halfScreenWidth = (Screen.width / 2f);
        ScreenTouchType touchType = ScreenTouchType.NONE;

        touchType = inputData.position.x < halfScreenWidth ?
                    ScreenTouchType.LEFT : ScreenTouchType.RIGHT;

        onTouch.Invoke(touchType);
    }
}
