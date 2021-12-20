using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

using DuDungTakGames.Input;

public class VMInputManager : MonoSingleton<VMInputManager>
{

    //public enum InputType { DEVICE, UI }

    //[SerializeField] // NOTE : 마우스, 기기 터치 조작 또는 UI 트리거 조작
    //InputType inputType;

    //[SerializeField]
    //EventTrigger trigger;

    public Dictionary<TouchPhase, UnityEvent<InputData>> inputEvents;
    public Dictionary<TouchPhase, InputData> inputDatas;

    bool isLockControl, isPointerObject;

#if UNITY_EDITOR
    Vector3 lastMousePosition;
#endif

    protected override void Init()
    {
        InitData();

        //if (inputType == InputType.UI)
        //{
        //    if (trigger)
        //    {
        //        InitTrigger();
        //    }
        //}
    }

    void InitData()
    {
        inputEvents = new Dictionary<TouchPhase, UnityEvent<InputData>>();
        inputDatas = new Dictionary<TouchPhase, InputData>();

        inputEvents.Add(TouchPhase.Began, new UnityEvent<InputData>());
        inputEvents.Add(TouchPhase.Moved, new UnityEvent<InputData>());
        inputEvents.Add(TouchPhase.Stationary, new UnityEvent<InputData>());
        inputEvents.Add(TouchPhase.Ended, new UnityEvent<InputData>());

        inputDatas.Add(TouchPhase.Began, new InputData());
        inputDatas.Add(TouchPhase.Moved, new InputData());
        inputDatas.Add(TouchPhase.Stationary, new InputData());
        inputDatas.Add(TouchPhase.Ended, new InputData());
    }

    //void InitTrigger()
    //{
    //    CreateEntry(trigger, EventTriggerType.BeginDrag, (data) =>
    //    {
    //        OnInputEvent(TouchPhase.Began, inputDatas[TouchPhase.Began].SetData(data));
    //    });

    //    CreateEntry(trigger, EventTriggerType.Move, (data) =>
    //    {
    //        OnInputEvent(TouchPhase.Moved, inputDatas[TouchPhase.Moved].SetData(data));
    //    });

    //    CreateEntry(trigger, EventTriggerType.Drag, (data) =>
    //    {
    //        OnInputEvent(TouchPhase.Stationary, inputDatas[TouchPhase.Stationary].SetData(data));
    //    });

    //    CreateEntry(trigger, EventTriggerType.BeginDrag, (data) =>
    //    {
    //        OnInputEvent(TouchPhase.Ended, inputDatas[TouchPhase.Ended].SetData(data));
    //    });
    //}

    void Update()
    {
        if (isLockControl)
            return;

#if !UNITY_EDITOR && !UNITY_STANDALONE
        UpdateTouch();
#else
        UpdateInput();
#endif
    }

    void UpdateInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            OnInputEvent(TouchPhase.Began, inputDatas[TouchPhase.Began].SetData(Input.mousePosition));
        }

        if (Input.GetMouseButton(0))
        {
            if (lastMousePosition == Input.mousePosition)
            {
                OnInputEvent(TouchPhase.Stationary, inputDatas[TouchPhase.Stationary].SetData(Input.mousePosition));
            }
            else
            {
                OnInputEvent(TouchPhase.Moved, inputDatas[TouchPhase.Moved].SetData(Input.mousePosition));
            }

            lastMousePosition = Input.mousePosition;
        }

        if (Input.GetMouseButtonUp(0))
        {
            OnInputEvent(TouchPhase.Ended, inputDatas[TouchPhase.Ended].SetData(Input.mousePosition));
        }
    }

#if !UNITY_EDITOR && !UNITY_STANDALONE
    void UpdateTouch()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            switch (touch.phase)
            {
                case TouchPhase.Began:
                    OnInputEvent(TouchPhase.Began, inputDatas[TouchPhase.Began].SetData(Input.mousePosition));
                    break;
                case TouchPhase.Moved:
                    OnInputEvent(TouchPhase.Moved, inputDatas[TouchPhase.Moved].SetData(Input.mousePosition));
                    break;
                case TouchPhase.Stationary:
                    OnInputEvent(TouchPhase.Stationary, inputDatas[TouchPhase.Stationary].SetData(Input.mousePosition));
                    break;
                case TouchPhase.Ended:
                    OnInputEvent(TouchPhase.Ended, inputDatas[TouchPhase.Ended].SetData(Input.mousePosition));
                    break;
                default:
                    break;
            }
        }
    }
#endif

    void OnInputEvent(TouchPhase touchPhase, InputData inputData)
    {
        if (IsPointerOverUIObject())
            return;

        inputEvents[touchPhase].Invoke(inputData);
    }

    bool IsPointerOverUIObject(bool isEndInput = false)
    {
        //if (inputType == InputType.UI)
        //    return false;

        if (GetPointerRayCount() > 0)
        {
            if (isEndInput)
                return true;

            isPointerObject = true;
        }

        return isPointerObject;
    }

    int GetPointerRayCount()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);

#if !UNITY_EDITOR && !UNITY_STANDALONE
        eventDataCurrentPosition.position = new Vector2(Input.GetTouch(0).position.x, Input.GetTouch(0).position.y);
#else
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
#endif

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);

        return results.Count;
    }

    public void SetControlLock(bool isOn)
    {
        StartCoroutine(SetControlLockCoroutine(isOn));
    }

    IEnumerator SetControlLockCoroutine(bool isOn)
    {
        yield return new WaitForEndOfFrame();

        isLockControl = isOn;
    }

    EventTrigger.Entry CreateEntry(EventTrigger trigger, EventTriggerType type, UnityAction<PointerEventData> action)
    {
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = type;
        entry.callback.AddListener((data) => { action((PointerEventData)data); });
        return entry;
    }
}
