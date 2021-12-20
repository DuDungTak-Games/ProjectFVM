using UnityEngine;
using UnityEngine.Events;

using DuDungTakGames.Input;

public class VMInputSwipe : MonoBehaviour
{

    VMInputManager inputManager;

    public enum SwipeTouchType 
    { 
        NONE,
        RIGHT, LEFT, UP, DOWN, 
        RIGHT_UP, LEFT_UP, 
        RIGHT_DOWN, LEFT_DOWN 
    }
    
    [HideInInspector]
    public UnityEvent<SwipeTouchType> onSwipe;
    
    [HideInInspector]
    public UnityEvent onTouch;
    
    [SerializeField]
    float maxTouchDistance = 10f;
    
    float minSwipeDistance;

    void Awake()
    {
        inputManager = VMInputManager.Instance;

        Init();
    }

    void Init()
    {
        Vector2 screenSize = new Vector2(Screen.width, Screen.height);
        minSwipeDistance = Mathf.Max(screenSize.x, screenSize.y) / 20f;

        inputManager.inputEvents[TouchPhase.Ended].AddListener(CheckMultiSwipe);
        inputManager.inputEvents[TouchPhase.Ended].AddListener(CheckTouch);
    }

    void OnDestroy()
    {
        inputManager.inputEvents[TouchPhase.Ended].RemoveListener(CheckMultiSwipe);
        inputManager.inputEvents[TouchPhase.Ended].RemoveListener(CheckTouch);
    }

    // NOTE : 4방향 스와이프
    // void CheckSingleSwipe(InputData inputData)
    // {
    //     Vector2 currentSwipe = endInputData.position - beginInputData.position;
    //
    //     if(currentSwipe.magnitude >= minSwipeDistance)
    //     {
    //         SwipeTouchType swipeType;
    //
    //         Vector2 swipeDirection = currentSwipe.normalized;
    //
    //         swipeType = swipeDirection.x > 0 ? SwipeTouchType.RIGHT : SwipeTouchType.LEFT;
    //
    //         if (Mathf.Abs(swipeDirection.y) > Mathf.Abs(swipeDirection.x))
    //         {
    //             swipeType = swipeDirection.y > 0 ? SwipeTouchType.UP : SwipeTouchType.DOWN;
    //         }
    //
    //         onSwipe.Invoke(swipeType);
    //     }
    // }

    // NOTE : 8방향 스와이프
    void CheckMultiSwipe(InputData inputData)
    {
        Vector2 currentSwipe = GetInputPosition(TouchPhase.Ended) - GetInputPosition(TouchPhase.Began);

        if (currentSwipe.magnitude >= minSwipeDistance)
        {
            SwipeTouchType swipeType = SwipeTouchType.NONE;

            Vector2 swipeDirection = currentSwipe.normalized;

            swipeType = swipeDirection.x > 0 ? SwipeTouchType.RIGHT : SwipeTouchType.LEFT;

            if (Mathf.Abs(swipeDirection.y) > Mathf.Abs(swipeDirection.x) / 2f)
            {
                if (swipeDirection.y > 0)
                {
                    if (Mathf.Abs(swipeDirection.x) <= 0.2f)
                    {
                        swipeType = SwipeTouchType.UP;
                    }
                    else
                    {
                        swipeType = (swipeType == SwipeTouchType.RIGHT) ? 
                                    SwipeTouchType.RIGHT_UP : SwipeTouchType.LEFT_UP;
                    }
                }
                else
                {
                    if (Mathf.Abs(swipeDirection.x) <= 0.2f)
                    {
                        swipeType = SwipeTouchType.DOWN;
                    }
                    else
                    {
                        swipeType = (swipeType == SwipeTouchType.RIGHT) ? 
                            SwipeTouchType.RIGHT_DOWN : SwipeTouchType.LEFT_DOWN;
                    }
                }
            }

            onSwipe.Invoke(swipeType);
        }
    }
    
    void CheckTouch(InputData inputData)
    {
        Vector2 currentSwipe = GetInputPosition(TouchPhase.Ended) - GetInputPosition(TouchPhase.Began);

        if (currentSwipe.magnitude < maxTouchDistance)
        {
            onTouch.Invoke();
        }
    }

    Vector3 GetInputPosition(TouchPhase touchPhase)
    {
        return inputManager.inputDatas[touchPhase].position;
    }
}
