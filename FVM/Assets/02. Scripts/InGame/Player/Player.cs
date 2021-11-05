using System;
using System.Collections;
using UnityEngine;

using DuDungTakGames.Extensions;

using SwipeType = VMInputSwipe.SwipeType;

public class Player : MonoBehaviour
{
    public Vector3 rayHeightOffset = new Vector3(0, 5, 0);

    public float moveUnit = 10f;
    public float heightUnit = 5f;

    public PlayerController pc;
    
    public VMInputSwipe vmInput;
    
    float currentFloor = 0;
    Floor directionFloor;
    GimicTrigger forwardGimic;

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
        vmInput?.onTouch.AddListener(Move);
        vmInput?.onSwipe.AddListener(Rotate);
        
        TestGameManager.Instance.SetGameEvent(GameState.COIN_GAME);
    }

    void ClearEvent()
    {
        vmInput?.onTouch.RemoveListener(Move);
        vmInput?.onSwipe.RemoveListener(Rotate);
    }

    void Rotate(SwipeType swipeType)
    {
        if (!TestGameManager.Instance.IsGameState(GameState.COIN_GAME))
            return;
        
        if (!pc.isRotating())
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
            
            pc.Rotate(direction);

            Move(direction);
        }
    }

    void Move()
    {
        if (!TestGameManager.Instance.IsGameState(GameState.COIN_GAME))
            return;
        
        if (!pc.isRotating())
        {
            Move(transform.forward);
        }
    }

    void Move(Vector3 direction)
    {
        if (!pc.isMoving())
        {
            directionFloor = null;

            if (CheckDirection(direction))
                return;

            if (!CheckFloor(direction))
                return;

            Vector3 movePos = transform.position + GetMoveUnit(direction) + GetMoveHeight();
            pc.Move(transform.position, movePos);
        }
    }

    bool CheckDirection(Vector3 direction)
    {
        RaycastHit hit;
        int layerMask = ~(1 << LayerMask.NameToLayer("Player") | 1 << LayerMask.NameToLayer("Item") |
                          1 << LayerMask.NameToLayer("Trigger"));
        if(Physics.Raycast(GetRayOrigin(), direction, out hit, 10f, layerMask))
        {
            GameObject obj = hit.collider.gameObject;
            if (obj.TryGetComponent(out directionFloor))
            {
                float result = (directionFloor.floor - currentFloor);
                if (Mathf.Abs(result) == 0.5f)
                    return false;
            }
            if (obj.TryGetComponent(out forwardGimic))
            {
                forwardGimic.OnTrigger();
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
