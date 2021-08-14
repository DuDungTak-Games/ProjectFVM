using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;

using DuDungTakGames.Input;

public class ScreenInputTest : MonoBehaviour, IInputHandler
{

    // TODO : InputTest Class 의 중복 필드, 메서드 정리 필요

    public enum ScreenTouchType { NONE = 0, RIGHT = 1, LEFT = -1 }

    InputData beginInputData;

    public UnityEvent<InputData> onBeginInput;

    public UnityEvent<ScreenTouchType> onTouch;

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
    }

    void InitEvent()
    {
        onBeginInput.AddListener(CheckScreenTouch);
    }

    void UpdateInput()
    {
        if (Input.GetMouseButton(0))
        {
            if (EventSystem.current.IsPointerOverGameObject(-1))
                return;

            beginInputData.SetData(Input.mousePosition);
            OnBeginInput(beginInputData);
        }
    }

    void UpdateTouch()
    {
        if (Input.touchCount > 0)
        {
            if (EventSystem.current.IsPointerOverGameObject(-1))
                return;

            Touch touch = Input.GetTouch(0);
            beginInputData.SetData(touch);
            OnBeginInput(beginInputData);
        }
    }

    public void OnBeginInput(InputData inputData)
    {
        onBeginInput.Invoke(inputData);
    }

    public void OnInput(InputData inputData)
    {

    }

    public void OnEndInput(InputData inputData)
    {

    }



    void CheckScreenTouch(InputData inputData)
    {
        ScreenTouchType touchType = ScreenTouchType.NONE;

        if (inputData .position.x < (Screen.width / 2))
        {
            touchType = ScreenTouchType.LEFT;
        }

        if (inputData.position.x > (Screen.width / 2))
        {
            touchType = ScreenTouchType.RIGHT;
        }

        onTouch.Invoke(touchType);
    }
}
