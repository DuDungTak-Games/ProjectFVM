using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DuDungTakGames.Event;

using SwipeType = VMInputSwipe.SwipeType;

public class PlayerTest : MonoBehaviour, IEventHandler
{
    public Vector3 rayHeightOffset = new Vector3(0, 5, 0);

    public float moveUnit = 10f;
    public float heightUnit = 5f;

    public VMInputSwipe vmInput;
    
    float currentFloor = 1f;
    Floor forwardFloor;

    Coroutine moveCoroutine;

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

        transform.rotation = Quaternion.LookRotation(direction, Vector3.up);

        Move();
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
        moveCoroutine = StartCoroutine(MoveCoroutine(movePos));
        //transform.position += GetMoveUnit() + GetMoveHeight();
    }

    IEnumerator MoveCoroutine(Vector3 movePos)
    {
        float progress = 0f;

        Vector3 startPos = transform.position;
        Vector3 startPosH = transform.position + (Vector3.up * 4f);

        Vector3 endPos = movePos;
        Vector3 endPosH = movePos + (Vector3.up * 4f);

        while (progress < 1f)
        {
            Vector3 A = Vector3.Lerp(startPos, startPosH, progress);
            Vector3 B = Vector3.Lerp(startPosH, endPosH, progress);
            Vector3 C = Vector3.Lerp(endPosH, endPos, progress);

            Vector3 D = Vector3.Lerp(A, B, progress);
            Vector3 E = Vector3.Lerp(B, C, progress);

            Vector3 F = Vector3.Lerp(D, E, progress);

            // NOTE : Lerp 에 0.1f 추가로 보정
            progress = Mathf.Lerp(progress, 1.1f, 15f * Time.deltaTime);

            transform.position = F;

            yield return new WaitForEndOfFrame();
        }

        transform.position = endPos;

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
