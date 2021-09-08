using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

using DuDungTakGames.Input;

public class VMInput : MonoBehaviour
    {
        
        public enum InputType { DEVICE, UI }

        [SerializeField] // NOTE : 마우스, 기기 터치 조작 또는 UI 트리거 조작
        InputType inputType;
        
        [SerializeField]
        EventTrigger trigger;
        
        [SerializeField] // NOTE : 디버깅용
        Image startCircle, currentCircle, endCircle;
        
        [HideInInspector]
        public UnityEvent<InputData> onBeginInput, onStayInput, onEndInput;

        protected InputData beginInputData, stayInputData, endInputData;

        bool isPointerObject;
        
        void Start()
        {
            Init();
        }

        public virtual void Init()
        {
            if (trigger && inputType == InputType.UI)
            {
                InitTrigger();
            }
            
            onBeginInput.AddListener(StartCircle);
            onStayInput.AddListener(MoveCircle);
            onEndInput.AddListener(EndCircle);
        }
        
        void InitTrigger()
        {
            CreateEntry(trigger, EventTriggerType.BeginDrag, (data) =>
            {
                beginInputData.SetData(data);
                OnBeginInput(beginInputData);
            });

            CreateEntry(trigger, EventTriggerType.Drag, (data) =>
            {
                stayInputData.SetData(data);
                OnStayInput(stayInputData);
            });

            CreateEntry(trigger, EventTriggerType.EndDrag, (data) =>
            {
                endInputData.SetData(data);
                OnEndInput(endInputData);
            });
        }

        EventTrigger.Entry CreateEntry(EventTrigger trigger, EventTriggerType type, UnityAction<PointerEventData> action)
        {
            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = type;
            entry.callback.AddListener((data) => { action((PointerEventData)data); });
            return entry;
        }

        void Update()
        {
            UpdateLogic();
        }

        public virtual void UpdateLogic()
        {
#if !UNITY_EDITOR && !UNITY_STANDALONE
            UpdateTouch();
#else
                UpdateInput();
#endif
        }
        
        void UpdateInput()
        {
            if(Input.GetMouseButtonDown(0))
            {
                beginInputData.SetData(Input.mousePosition);
                OnBeginInput(beginInputData);
            }

            if (Input.GetMouseButton(0))
            {
                stayInputData.SetData(Input.mousePosition);
                OnStayInput(stayInputData);
            }

            if (Input.GetMouseButtonUp(0))
            {
                endInputData.SetData(Input.mousePosition);
                OnEndInput(endInputData);
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
                        beginInputData.SetData(touch);
                        OnBeginInput(beginInputData);
                        break;
                    case TouchPhase.Stationary:
                        stayInputData.SetData(touch);
                        OnStayInput(stayInputData);
                        break;
                    case TouchPhase.Ended:
                        endInputData.SetData(touch);
                        OnEndInput(endInputData);
                        break;
                    default:
                        break;
                }
            }
        }
#endif

        protected bool IsPointerOverUIObject(bool isEndInput = false)
        {
            if (inputType == InputType.UI)
                return false;

            if (GetPointerRayCount() > 0)
            {
                if(isEndInput)
                    return true;

                isPointerObject = true;
            }
            
            return isPointerObject;
        }

        protected void OnBeginInput(InputData inputData)
        {
            if (IsPointerOverUIObject())
                return;

            onBeginInput.Invoke(inputData);
        }

        protected void OnStayInput(InputData inputData)
        {
            if (IsPointerOverUIObject())
                return;

            onStayInput.Invoke(inputData);
        }

        protected void OnEndInput(InputData inputData)
        {
            isPointerObject = false;
            
            if (IsPointerOverUIObject(true))
                return;

            onEndInput.Invoke(inputData);
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

        void StartCircle(InputData inputData)
        {
            if (startCircle == null) return;
            startCircle.transform.position = inputData.position;
        }

        void MoveCircle(InputData inputData)
        {
            if (currentCircle == null) return;
            currentCircle.transform.position = inputData.position;
        }

        void EndCircle(InputData inputData)
        {
            if (endCircle == null) return;
            endCircle.transform.position = inputData.position;
        }
    }