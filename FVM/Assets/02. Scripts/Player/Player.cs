using System;
using System.Collections;
using UnityEngine;

using DuDungTakGames.Extensions;

using SwipeType = VMInputSwipe.SwipeType;

public class Player : MonoBehaviour
{
    public Vector3 rayHeightOffset = new Vector3(0, 5, 0);

    public float moveUnit = 10f, moveSpeed = 16f;
    public float rotateSpeed = 20f;
    public float heightUnit = 5f;

    public VMInputSwipe vmInput;
    
    float currentFloor = 1f;
    Floor directionFloor;

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
            
            RotateCoroutine(direction).Start(ref rotateCoroutine, this);

            Move(direction);
        }
    }
    
    IEnumerator RotateCoroutine(Vector3 direction)
    {
        Quaternion startRot = transform.rotation;
        Quaternion endRot = Quaternion.LookRotation(direction, Vector3.up);

        yield return ProcessAction(rotateSpeed, (t) =>
        {
            transform.rotation = Quaternion.Lerp(startRot, endRot, t);
        });

        transform.SetRotation(endRot);

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
            directionFloor = null;

            if (CheckDirection(direction))
                return;

            if (!CheckFloor(direction))
                return;

            Vector3 movePos = transform.position + GetMoveUnit(direction) + GetMoveHeight();
            MoveCoroutine(transform.position, movePos).Start(ref moveCoroutine, this);
        }
    }

    IEnumerator MoveCoroutine(Vector3 startPos, Vector3 movePos)
    {
        yield return ProcessAction(moveSpeed, (t) =>
        {
            transform.BezierCurvePosition(startPos, movePos, (Vector3.up * 4f), t);
        });

        transform.SetPosition(movePos);

        moveCoroutine = null;
    }

    // TODO : 추후에 코루틴 확장 or 매니저 클래스에 넣기
    // NOTE : 이동하거나 회전이 필요한 오브젝트에 자주 쓰일 것으로 예상됨
    IEnumerator ProcessAction(float speed, Action<float> action)
    {
        float progress = 0f;

        while (progress < 1f)
        {
            action(progress);

            // NOTE : Lerp 에 0.1f 추가로 보정
            progress = Mathf.Lerp(progress, 1.1f, speed * Time.deltaTime);

            yield return new WaitForEndOfFrame();
        }
    }

    bool CheckDirection(Vector3 direction)
    {
        RaycastHit hit;
        if(Physics.Raycast(GetRayOrigin(), direction, out hit, 10f, ~(1 << LayerMask.NameToLayer("Player") | 1 << LayerMask.NameToLayer("Item"))))
        {
            GameObject obj = hit.collider.gameObject;
            if (obj.TryGetComponent(out directionFloor))
            {
                float result = (directionFloor.floor - currentFloor);
                if (Mathf.Abs(result) == 0.5f)
                    return false;
            }
            return true;
        }
        return false;
    }

    bool CheckFloor(Vector3 direction)
    {
        RaycastHit hit;
        if (Physics.Raycast(GetRayOrigin() + (direction * 10f), Vector3.down, out hit, 11f, (1 << LayerMask.NameToLayer("Tile"))))
        {
            GameObject obj = hit.collider.gameObject;
            return obj.TryGetComponent(out directionFloor);
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
        if (directionFloor == null)
            return Vector3.zero;

        float targetFloor = directionFloor.floor;
        float result = (targetFloor - currentFloor);

        float posY = 0;
        if (Mathf.Abs(result) == 0.5f)
            posY = result < 0 ? -heightUnit : heightUnit;

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
