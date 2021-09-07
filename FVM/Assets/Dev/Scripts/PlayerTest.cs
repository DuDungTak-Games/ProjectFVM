using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DuDungTakGames.Event;
using DuDungTakGames.Extensions;

using SwipeType = VMInputSwipe.SwipeType;

public class PlayerTest : MonoBehaviour, IEventHandler
{
    public Vector3 rayHeightOffset = new Vector3(0, 5, 0);

    public float moveUnit = 10f, moveSpeed = 16f;
    public float rotateSpeed = 20f;
    public float heightUnit = 5f;

    public VMInputSwipe vmInput;
    
    float currentFloor = 1f;
    Floor forwardFloor;

    Coroutine moveCoroutine, rotateCoroutine;

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
        vmInput.onSwipe.AddListener(Rotate);
        vmInput.onTouch.AddListener(Move);
    }

    public void ClearEvent()
    {
        vmInput.onSwipe.RemoveListener(Rotate);
        vmInput.onTouch.RemoveListener(Move);
    }

    void Rotate(SwipeType swipeType)
    {
        Vector3 direction = Vector3.zero;

        switch (swipeType)
        {
            case SwipeType.RIGHT:
                direction = Vector3.right;
                break;
            case SwipeType.LEFT:
                direction = Vector3.left;
                break;
            case SwipeType.UP:
                direction = Vector3.forward;
                break;
            case SwipeType.DOWN:
                direction = Vector3.back;
                break;
            case SwipeType.RIGHT_UP:
                direction = Vector3.forward;
                break;
            case SwipeType.LEFT_UP:
                direction = Vector3.left;
                break;
            case SwipeType.RIGHT_DOWN:
                direction = Vector3.right;
                break;
            case SwipeType.LEFT_DOWN:
                direction = Vector3.back;
                break;
            default:
                return;
        }

        if (rotateCoroutine != null)
            return;

        rotateCoroutine = StartCoroutine(RotateCoroutine(direction));

        //Move();
    }
    
    IEnumerator RotateCoroutine(Vector3 direction)
    {
        float progress = 0f;

        Quaternion startRot = transform.localRotation;
        Quaternion endRot = Quaternion.LookRotation(direction, Vector3.up);

        while (progress < 1f)
        {
            transform.localRotation = Quaternion.Lerp(startRot, endRot, progress);

            // NOTE : Lerp 에 0.1f 추가로 보정
            progress = Mathf.Lerp(progress, 1.1f, rotateSpeed * Time.deltaTime);

            yield return new WaitForEndOfFrame();
        }

        transform.localRotation = endRot;

        rotateCoroutine = null;
        
        Move();

        yield break;
    }

    void Move()
    {
        if (moveCoroutine != null)
            return;

        forwardFloor = null;

        if (CheckFoward())
            return;

        if (!CheckFloor())
            return;

        Vector3 movePos = transform.position + GetMoveUnit() + GetMoveHeight();
        moveCoroutine = StartCoroutine(MoveCoroutine(transform.position, movePos));
    }

    IEnumerator MoveCoroutine(Vector3 startPos, Vector3 movePos)
    {
        float progress = 0f;

        while (progress < 1f)
        {
            transform.BezierCurvePosition(startPos, movePos, (Vector3.up * 4f), progress);

            // NOTE : Lerp 에 0.1f 추가로 보정
            progress = Mathf.Lerp(progress, 1.1f, moveSpeed * Time.deltaTime);

            yield return new WaitForEndOfFrame();
        }

        transform.position = movePos;

        moveCoroutine = null;

        yield break;
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
        if (Physics.Raycast(GetRayOrigin() + (transform.forward * 10f), Vector3.down, out hit, 11f, (1 << LayerMask.NameToLayer("Tile"))))
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
