using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DuDungTakGames.Event;

using SwipeType = InputTest.SwipeType;

public class PlayerTest : MonoBehaviour, IEventHandler
{

    public InputTest inputTest;

    public Vector3 rayHeightOffset = new Vector3(0, 5, 0);

    public float moveUnit = 10f;
    public float heightUnit = 5f;

    float currentFloor = 1f;
    Floor forwardFloor;

    void Awake()
    {
        Init();
    }

    void Init()
    {
        SetEvent();
    }

    public void SetEvent()
    {
        inputTest.onSwipe.AddListener(Rotate);
        inputTest.onTouch.AddListener(Move);
    }

    public void ClearEvent()
    {
        inputTest.onSwipe.RemoveListener(Rotate);
        inputTest.onTouch.RemoveListener(Move);
    }



    // TODO : 부드러운 회전 모션 적용
    void Rotate(SwipeType swipeType)
    {
        //Vector3 direction = Vector3.zero;
        int angle = 0;

        switch (swipeType)
        {
            case SwipeType.LEFT:
                //direction = Vector3.left;
                angle = -45;
                break;
            case SwipeType.RIGHT:
                //direction = Vector3.right;
                angle = 45;
                break;
            case SwipeType.UP:
                //direction = Vector3.forward;
                break;
            case SwipeType.DOWN:
                //direction = Vector3.back;
                break;
            default:
                break;
        }

        // transform.rotation = Quaternion.LookRotation(direction, Vector3.up);
        transform.Rotate(Vector3.up, angle * 90);
    }

    // TODO : 부드러운 이동 모션 적용
    void Move()
    {
        forwardFloor = null;

        if (CheckFoward())
            return;

        if (!CheckFloor())
            return;

        transform.position += GetMoveUnit() + GetMoveHeight();
    }

    bool CheckFoward()
    {
        RaycastHit hit;
        if(Physics.Raycast(GetRayOrigin(), transform.forward, out hit, 10f, ~(1 << LayerMask.NameToLayer("Player") | 1 << LayerMask.NameToLayer("Item"))))
        {
            GameObject hitObj = hit.collider.gameObject;

            if(hitObj)
            {
                if (hitObj.TryGetComponent(out forwardFloor))
                {
                    float result = (forwardFloor.floor - currentFloor);
                    if (Mathf.Abs(result) != 0.5f)
                    {
                        return true;
                    }

                    return false;
                }

                return true;
            }
        }

        return false;
    }

    bool CheckFloor()
    {
        RaycastHit hit;
        if (Physics.Raycast(GetRayOrigin() + (transform.forward * 10f), Vector3.down, out hit, 10f, ~(1 << LayerMask.NameToLayer("Player") | 1 << LayerMask.NameToLayer("Item"))))
        {
            GameObject hitObj = hit.collider.gameObject;

            if (hitObj)
            {
                if(forwardFloor == null)
                {
                    return hitObj.TryGetComponent(out forwardFloor);
                }

                return true;
            }
        }

        return false;
    }

    Vector3 GetRayOrigin()
    {
        return (transform.position + rayHeightOffset);
    }

    Vector3 GetMoveUnit()
    {
        return (transform.forward * moveUnit);
    }

    Vector3 GetMoveHeight()
    {
        if (forwardFloor == null)
            return Vector3.zero;

        float targetFloor = forwardFloor.floor;
        float result = (targetFloor - currentFloor);

        float posY = 0;
        if (Mathf.Abs(result) == 0.5f)
        {
            posY = result < 0 ? -heightUnit : heightUnit;
        }

        currentFloor = targetFloor;

        return new Vector3(0, posY, 0);
    }



    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawRay(transform.position + rayHeightOffset, transform.forward * 10f);       
        Gizmos.DrawRay(transform.position + rayHeightOffset + (transform.forward * 10f), Vector3.down * 10f);       
    }
}
