using UnityEngine;
using UnityEngine.Events;

using DuDungTakGames.Input;

public class VMInputScreen : VMInput
{
    
    public enum ScreenTouchType
    {
        NONE = 0, 
        RIGHT = 1, 
        LEFT = -1
    }
    
    [HideInInspector]
    public UnityEvent<ScreenTouchType> onTouch;

    public override void Init()
    {
        base.Init();
        
        onStayInput.AddListener(CheckScreenTouch);
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
