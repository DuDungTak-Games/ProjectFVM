using UnityEngine;
using UnityEngine.Events;

using DuDungTakGames.Input;

public class VMInputSwipe : VMInput
{
    
    public enum SwipeType 
    { 
        RIGHT, LEFT, UP, DOWN, 
        RIGHT_UP, LEFT_UP, 
        RIGHT_DOWN, LEFT_DOWN 
    }
    
    [HideInInspector]
    public UnityEvent<SwipeType> onSwipe;
    
    [HideInInspector]
    public UnityEvent onTouch;
    
    [SerializeField]
    float maxTouchDistance = 10f;
    
    float minSwipeDistance;

    void Awake()
    {
        Vector2 screenSize = new Vector2(Screen.width, Screen.height);
        minSwipeDistance = Mathf.Max(screenSize.x, screenSize.y) / 20f;

        Debug.LogWarningFormat("MinSwipeDistance : {0}", minSwipeDistance);
    }

    public override void Init()
    {
        base.Init();
        
        onEndInput.AddListener(CheckMultiSwipe);
        onEndInput.AddListener(CheckTouch);
    }
    
    // void CheckSingleSwipe(InputData inputData)
    // {
    //     Vector2 currentSwipe = endInputData.position - beginInputData.position;
    //
    //     if(currentSwipe.magnitude >= minSwipeDistance)
    //     {
    //         SwipeType swipeType;
    //
    //         Vector2 swipeDirection = currentSwipe.normalized;
    //
    //         swipeType = swipeDirection.x > 0 ? SwipeType.RIGHT : SwipeType.LEFT;
    //
    //         if (Mathf.Abs(swipeDirection.y) > Mathf.Abs(swipeDirection.x))
    //         {
    //             swipeType = swipeDirection.y > 0 ? SwipeType.UP : SwipeType.DOWN;
    //         }
    //
    //         onSwipe.Invoke(swipeType);
    //     }
    // }
        
    void CheckMultiSwipe(InputData inputData)
    {
        Vector2 currentSwipe = endInputData.position - beginInputData.position;

        if(currentSwipe.magnitude >= minSwipeDistance)
        {
            SwipeType swipeType;

            Vector2 swipeDirection = currentSwipe.normalized;
            Debug.LogWarningFormat("SwipeDirection : {0}", swipeDirection);

            swipeType = swipeDirection.x > 0 ? SwipeType.RIGHT : SwipeType.LEFT;

            if (Mathf.Abs(swipeDirection.y) > Mathf.Abs(swipeDirection.x) / 2f)
            {
                if (swipeDirection.y > 0)
                {
                    if (Mathf.Abs(swipeDirection.x) <= 0.2f)
                    {
                        swipeType = SwipeType.UP;
                    }
                    else
                    {
                        swipeType = (swipeType == SwipeType.RIGHT) ? 
                                    SwipeType.RIGHT_UP : SwipeType.LEFT_UP;
                    }
                }
                else
                {
                    if (Mathf.Abs(swipeDirection.x) <= 0.2f)
                    {
                        swipeType = SwipeType.DOWN;
                    }
                    else
                    {
                        swipeType = (swipeType == SwipeType.RIGHT) ? 
                            SwipeType.RIGHT_DOWN : SwipeType.LEFT_DOWN;
                    }
                }
            }

            onSwipe.Invoke(swipeType);
        }
    }
    
    void CheckTouch(InputData inputData)
    {
        Vector2 currentSwipe = endInputData.position - beginInputData.position;

        if (currentSwipe.magnitude < maxTouchDistance)
        {
            onTouch.Invoke();
        }
    }
}
