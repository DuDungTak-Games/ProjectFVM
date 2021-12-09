using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DuDungTakGames.Extensions;

using SwipeType = VMInputSwipe.SwipeType;

public class PlayerTest : MonoBehaviour
{
    public Vector3 rayHeightOffset = new Vector3(0, 5, 0);

    public float moveUnit = 10f, moveSpeed = 16f;
    public float rotateSpeed = 20f;
    public float heightUnit = 5f;

    public VMInputSwipe vmInput;
    
    float currentFloor = 1f;
    Tile forwardTile;

    Coroutine moveCoroutine, rotateCoroutine;

    void Awake()
    {
        Init();
    }

    void Init()
    {
        SetEvent();
    }

    void SetEvent()
    {
        vmInput?.onSwipe.AddListener(Rotate);
        vmInput?.onTouch.AddListener(Move);
    }

    void ClearEvent()
    {
        vmInput?.onSwipe.RemoveListener(Rotate);
        vmInput?.onTouch.RemoveListener(Move);
    }

    void Rotate(SwipeType swipeType)
    {
        if (rotateCoroutine == null)
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
            
            rotateCoroutine = RotateCoroutine(direction).Start(this);

            Move(direction);
        }
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
    }

    void Move()
    {
        if (rotateCoroutine == null)
        {
            Move(transform.forward);
        }
    }

    void Move(Vector3 direction)
    {
        if (moveCoroutine == null)
        {
            forwardTile = null;

            if (CheckFoward(direction))
                return;

            if (!CheckFloor(direction))
                return;

            Vector3 movePos = transform.position + GetMoveUnit(direction) + GetMoveHeight();
            moveCoroutine = MoveCoroutine(transform.position, movePos).Start(this);
        }
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
    }

    bool CheckFoward(Vector3 direction)
    {
        RaycastHit hit;
        if(Physics.Raycast(GetRayOrigin(), direction, out hit, 10f, ~(1 << LayerMask.NameToLayer("Player") | 1 << LayerMask.NameToLayer("Item"))))
        {
            GameObject hitObj = hit.collider.gameObject;

            if(hitObj)
            {
                if (hitObj.TryGetComponent(out forwardTile))
                {
                    float result = (forwardTile.floor - currentFloor);
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

    bool CheckFloor(Vector3 direction)
    {
        RaycastHit hit;
        if (Physics.Raycast(GetRayOrigin() + (direction * 10f), Vector3.down, out hit, 11f, (1 << LayerMask.NameToLayer("Tile"))))
        {
            GameObject hitObj = hit.collider.gameObject;

            if (hitObj)
            {
                if(forwardTile == null)
                {
                    return hitObj.TryGetComponent(out forwardTile);
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

    Vector3 GetMoveUnit(Vector3 direction)
    {
        return (direction * moveUnit);
    }

    Vector3 GetMoveHeight()
    {
        if (forwardTile == null)
            return Vector3.zero;

        float targetFloor = forwardTile.floor;
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
