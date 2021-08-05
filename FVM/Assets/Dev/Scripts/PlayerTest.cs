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
        inputTest.onSwipe.AddListener(Rotate);
        inputTest.onTouch.AddListener(Move);
    }

    public void ClearEvent()
    {
        inputTest.onSwipe.RemoveListener(Rotate);
        inputTest.onTouch.RemoveListener(Move);
    }

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

            // NOTE : b 에 0.1f 추가로 보정
            progress = Mathf.Lerp(progress, 1.1f, 0.125f);

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
        if (Physics.Raycast(GetRayOrigin() + (transform.forward * 10f), Vector3.down, out hit, 10f, (1 << LayerMask.NameToLayer("Tile"))))
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
