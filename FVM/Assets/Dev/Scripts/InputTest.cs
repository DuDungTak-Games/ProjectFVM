using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;

using DuDungTakGames.Input;

public class InputTest : MonoBehaviour, IInputHandler
{

    public enum SwipeType { RIGHT, LEFT, UP, DOWN }

    //public EventTrigger trigger;

    public Image startCircle, currentCircle, endCircle;

    InputData beginInputData, inputData, endInputData;

    public UnityEvent<InputData> onBeginInput;
    public UnityEvent<InputData> onInput;
    public UnityEvent<InputData> onEndInput;

    public UnityEvent<SwipeType> onSwipe;
    public UnityEvent onTouch;

    float minSwipeDistance, maxTouchDistance;

    void Awake()
    {
        Vector2 screenSize = new Vector2(Screen.width, Screen.height);
        minSwipeDistance = Mathf.Max(screenSize.x, screenSize.y) / 12f;
        maxTouchDistance = 10f;

        Debug.LogWarningFormat("MinSwipeDistance : {0}", minSwipeDistance);
    }

    void Start()
    {
        Init();
    }

    void Update()
    {
        UpdateTouch();
        UpdateInput();
    }

    void Init()
    {
        InitEvent();
        //InitTrigger();
    }

    void InitEvent()
    {
        onBeginInput.AddListener(StartCircle);

        onInput.AddListener(MoveCircle);

        onEndInput.AddListener(EndCircle);

        onEndInput.AddListener(CheckSwipe);
        onEndInput.AddListener(CheckTouch);
    }

    //void InitTrigger()
    //{
    //    CreateEntry(trigger, EventTriggerType.BeginDrag, (data) =>
    //    {
    //        beginInputData.SetData(data);
    //        OnBeginInput(beginInputData);
    //    });

    //    CreateEntry(trigger, EventTriggerType.Drag, (data) =>
    //    {
    //        inputData.SetData(data);
    //        OnInput(inputData);
    //    });

    //    CreateEntry(trigger, EventTriggerType.EndDrag, (data) =>
    //    {
    //        endInputData.SetData(data);
    //        OnEndInput(endInputData);
    //    });
    //}

    //EventTrigger.Entry CreateEntry(EventTrigger trigger, EventTriggerType type, UnityAction<PointerEventData> action)
    //{
    //    EventTrigger.Entry entry = new EventTrigger.Entry();
    //    entry.eventID = type;
    //    entry.callback.AddListener((data) => { action((PointerEventData)data); });
    //    return entry;
    //}

    void UpdateInput()
    {
        if(Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current.IsPointerOverGameObject(-1))
                return;

            beginInputData.SetData(Input.mousePosition);
            OnBeginInput(beginInputData);
        }

        if (Input.GetMouseButton(0))
        {
            if (EventSystem.current.IsPointerOverGameObject(-1))
                return;

            inputData.SetData(Input.mousePosition);
            OnInput(inputData);
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (EventSystem.current.IsPointerOverGameObject(-1))
                return;

            endInputData.SetData(Input.mousePosition);
            OnEndInput(endInputData);
        }
    }

    void UpdateTouch()
    {
        if (Input.touchCount > 0)
        {
            if (EventSystem.current.IsPointerOverGameObject(-1))
                return;

            Touch touch = Input.GetTouch(0);
            switch (touch.phase)
            {
                case TouchPhase.Began:
                    beginInputData.SetData(touch);
                    OnBeginInput(beginInputData);
                    break;
                case TouchPhase.Moved:
                    inputData.SetData(touch);
                    OnInput(inputData);
                    break;
                case TouchPhase.Ended:
                    endInputData.SetData(touch);
                    OnEndInput(inputData);
                    break;
                default:
                    break;
            }
        }
    }

    public void OnBeginInput(InputData inputData)
    {
        onBeginInput.Invoke(inputData);
    }

    public void OnInput(InputData inputData)
    {
        onInput.Invoke(inputData);
    }

    public void OnEndInput(InputData inputData)
    {
        onEndInput.Invoke(inputData);
    }



    void CheckSwipe(InputData inputData)
    {
        Vector2 currentSwipe = endInputData.position - beginInputData.position;

        if(currentSwipe.magnitude >= minSwipeDistance)
        {
            SwipeType swipeType;

            Vector2 swipeDirection = currentSwipe.normalized;
            Debug.LogWarningFormat("SwipeDirection : {0}", swipeDirection);

            if (swipeDirection.x > 0)
            {
                swipeType = SwipeType.RIGHT;
            }
            else
            {
                swipeType = SwipeType.LEFT;
            }

            if (Mathf.Abs(swipeDirection.y) > Mathf.Abs(swipeDirection.x))
            {
                if (swipeDirection.y > 0)
                {
                    swipeType = SwipeType.UP;
                }
                else
                {
                    swipeType = SwipeType.DOWN;
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



    void StartCircle(InputData inputData)
    {
        startCircle.transform.position = inputData.position;
    }

    void MoveCircle(InputData inputData)
    {
        currentCircle.transform.position = inputData.position;
    }

    void EndCircle(InputData inputData)
    {
        endCircle.transform.position = inputData.position;
    }
}