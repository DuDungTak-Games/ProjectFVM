using System;
using System.Collections;
using UnityEngine;

using DuDungTakGames.Extensions;

using SwipeType = VMInputSwipe.SwipeType;

[RequireComponent(typeof(Raycaster))]
public class PlayerController : MonoBehaviour
{
    
    bool lockControl;
    
    [Header("Tile Move & Rotate")]
    [SerializeField] float moveUnit = 10f;
    [SerializeField] float heightUnit = 5f;
    
    [Space(10)]
    [SerializeField] float moveSpeed = 14f;
    [SerializeField] float rotateSpeed = 14f;
    [SerializeField] Vector3 moveHeight = new Vector3(0, 5, 0);

    [Header("Pit Move")]
    [SerializeField] float pitSpeed = 10f;
    [SerializeField] Vector3 pitMoveHeight = new Vector3(0, 20, 0);
    [SerializeField] Vector3 pitHeight = new Vector3(0, -20, 0);

    [Header("Jump Move")]
    [SerializeField] float jumpSpeed = 4f;
    [SerializeField] Vector3 jumpHeight = new Vector3(0, 60, 0);

    float curFloor = 0;

    Raycaster raycaster;

    Animator animator;

    VMInputSwipe vmInput;
    
    Tile dirTile;
    GimicTrigger dirTrigger;

    Action onMoveStart, onMoveEnd, onRotateEnd;
    Coroutine moveCoroutine, rotateCoroutine;

    void Awake()
    {
        Init();
    }
    
    void Init()
    {
        raycaster = this.GetComponent<Raycaster>();

        animator = this.GetComponent<Animator>();

        vmInput = FindObjectOfType<VMInputSwipe>();
        vmInput.onTouch.AddListener(Move);
        vmInput.onSwipe.AddListener(Rotate);
        
        AddMoveStartEvent(ResetDirTrigger);
        AddMoveEndEvent(FindNearTrigger);
        AddRotateEndEvent(FindNearTrigger);
    }

    
    
    public void AddMoveStartEvent(Action action)
    {
        onMoveStart += action;
    }
    
    public void AddMoveEndEvent(Action action)
    {
        onMoveEnd += action;
    }

    public void AddRotateEndEvent(Action action)
    {
        onRotateEnd += action;
    }

    void ClearEvent()
    {
        vmInput.onTouch.RemoveListener(Move);
        vmInput.onSwipe.RemoveListener(Rotate);

        onMoveStart = null;
        onMoveEnd = null;
        onRotateEnd = null;
    }
    
    void OnDestroy()
    {
        ClearEvent();
    }
    


    bool IsLockControl()
    {
        if (!TestGameManager.Instance.IsGameState(GameState.COIN_GAME))
            return true;

        return lockControl;
    }
    
    bool IsMoving()
    {
        return moveCoroutine != null || IsLockControl();
    }
    
    bool IsRotating()
    {
        return rotateCoroutine != null || IsLockControl();
    }

    void Move(Vector3 direction)
    {
        if (!IsMoving())
        {
            dirTile = null;

            if (CheckDirection(direction))
                return;

            if (!CheckDirectionFloor(direction))
                return;

            Vector3 movePos = GetPosUnit(transform.position) + GetMoveUnit(direction) + GetMoveHeight();
            MoveCoroutine(transform.position, movePos).Start(ref moveCoroutine, this);
        }
    }
    
    void Move()
    {
        if (!IsRotating())
        {
            Move(transform.forward);
        }
    }

    public void PitMove(GimicPit startPit, GimicPit endPit, Vector3 exitDir)
    {
        if(!IsMoving())
        {
            PitMoveCoroutine(startPit, endPit, exitDir).Start(ref moveCoroutine, this);
        }
    }

    public void JumpMove(Vector3 targetPos)
    {
        JumpMoveCoroutine(targetPos).Start(ref moveCoroutine, this);
    }

    void Rotate(SwipeType swipeType)
    {
        if (!IsRotating())
        {
            Vector3 targetDir = GetSwipeDirection(swipeType);
            RotateCoroutine(targetDir).Start(ref rotateCoroutine, this);
            Move(targetDir);
        }
    }

    // NOTE: 이동 방향에 타일 오브젝트 체크
    bool CheckDirection(Vector3 direction)
    {
        if (CheckTrigger(direction))
        {
            OnTrigger();
            return true;
        }
        
        GameObject rayObj = raycaster.CheckDirection(direction);
        if (rayObj)
        {
            if (rayObj.TryGetComponent(out dirTile))
            {
                if(dirTile.tileID is TileID.BOX)
                    Debug.Log(dirTile.floor - curFloor);
                float result = (dirTile.floor - curFloor);
                if (Mathf.Abs(result) == 0.5f)
                    return false;
                return true;
            }
        }

        return false;
    }

    // NOTE : 이동 방향 바닥에 타일 오브젝트 체크
    bool CheckDirectionFloor(Vector3 direction)
    {
        if (CheckTriggerFloor(direction))
        {
            return true;
        }

        int layerMask = (1 << LayerMask.NameToLayer("Tile"));

        GameObject rayObj = raycaster.CheckRayOrigin((direction * moveUnit), Vector3.down, 1f, layerMask);
        if (rayObj)
        {
            return (rayObj.TryGetComponent(out dirTile));
        }

        return false;
    }
    
    // NOTE : 상호작용 기믹 트리거 체크
    bool CheckTrigger(Vector3 direction)
    {
        GameObject rayObj = raycaster.CheckTrigger(direction);
        if (rayObj)
        {
            // NOTE : Floor 와 상관없이 트리거 작동됨
            return (rayObj.TryGetComponent(out dirTrigger));
        }

        return false;
    }

    bool CheckTriggerFloor(Vector3 direction)
    {
        GameObject rayObj = raycaster.CheckTriggerFloor(direction);
        if (rayObj)
        {
            if (rayObj.TryGetComponent(out dirTrigger))
            {
                if (dirTrigger is GimicBox)
                {
                    return (rayObj.TryGetComponent(out dirTile));
                }
                if (dirTrigger is GimicPit)
                {
                    OnTrigger();
                }
            }
        }

        return false;
    }


    void FindNearTrigger()
    {
        if(CheckTrigger(transform.forward)) return;
        if(CheckTrigger(transform.right)) return;
        if(CheckTrigger(-transform.right)) return;
    }

    void ResetDirTrigger()
    {
        dirTrigger = null;
    }

    public void OnTrigger()
    {
        if (dirTrigger)
        {
            dirTrigger.OnTrigger();
        }
        
        ResetDirTrigger();
        FindNearTrigger();
    }

    // NOTE : 일반 이동으로 변경된게 아닌 경우에는 -1 해줌
    public void SetFloor(float newFloor, bool isTileFloor = false)
    {
        curFloor = newFloor + (isTileFloor ? -1 : 0);
    }

    public void SetLockControl(bool isOn)
    {
        lockControl = isOn;
    }

    public Vector3 GetPosUnit(Vector3 pos)
    {
        float x = (int)Mathf.Round(pos.x/moveUnit) * moveUnit;
        float y = (int)Mathf.Round(pos.y/heightUnit) * heightUnit;
        float z = (int)Mathf.Round(pos.z/moveUnit) * moveUnit;

        return new Vector3(x, y, z);
    }

    public Vector3 GetMoveUnit(Vector3 direction)
    {
        return (direction * moveUnit);
    }

    Vector3 GetMoveHeight()
    {
        if (dirTile != null)
        {
            float targetFloor = dirTile.floor;
            float result = (targetFloor - curFloor);

            float posY = 0;
            if (Mathf.Abs(result) == 0.5f)
            {
                posY = Mathf.Sign(result) * heightUnit;
            }

            SetFloor(targetFloor);

            return new Vector3(0, posY, 0);
        }

        return Vector3.zero;
    }

    Vector3 GetSwipeDirection(SwipeType swipeType)
    {
        switch (swipeType)
        {
            case SwipeType.RIGHT:
                return Vector3.right;
            case SwipeType.LEFT:
                return Vector3.left;
            case SwipeType.UP:
                return Vector3.forward;
            case SwipeType.DOWN:
                return Vector3.back;
            case SwipeType.RIGHT_UP:
                return Vector3.forward;
            case SwipeType.LEFT_UP:
                return Vector3.left;
            case SwipeType.RIGHT_DOWN:
                return Vector3.right;
            case SwipeType.LEFT_DOWN:
                return Vector3.back;
            default:
                return Vector3.zero;
        }
    }

    
    
    IEnumerator MoveCoroutine(Vector3 startPos, Vector3 movePos)
    {
        onMoveStart?.Invoke();
        
        yield return CoroutineExtensions.ProcessAction(moveSpeed, (t) =>
        {
            transform.BezierCurvePosition(startPos, movePos, moveHeight, t);
        });

        transform.SetPosition(movePos);

        onMoveEnd?.Invoke();
        
        moveCoroutine = null;
    }
    
    IEnumerator RotateCoroutine(Vector3 direction)
    {
        Quaternion startRot = transform.rotation;
        Quaternion endRot = Quaternion.LookRotation(direction, Vector3.up);

        yield return CoroutineExtensions.ProcessAction(rotateSpeed, (t) =>
        {
            transform.rotation = Quaternion.Lerp(startRot, endRot, t);
        });

        transform.SetRotation(endRot);

        onRotateEnd?.Invoke();
        
        rotateCoroutine = null;
    }

    IEnumerator PitMoveCoroutine(GimicPit startPit, GimicPit endPit, Vector3 exitDir)
    {
        startPit.PlayTriggerAnim();

        animator.SetBool("isFloating", true);

        SetLockControl(true);

        Vector3 startPos = transform.position;
        Vector3 endPos = GetPosUnit(startPit.transform.position + pitHeight);

        yield return AccelerationMoveLogic(startPos, endPos, pitMoveHeight, pitSpeed);

        yield return new WaitForSeconds(0.5f);

        endPit.PlayTriggerAnim();

        SetFloor(endPit.tile.floor, true);
        dirTile = endPit.exitTile;

        startPos = (endPit.transform.position + pitHeight);
        endPos = GetPosUnit(endPit.transform.position) + GetMoveUnit(exitDir) + GetMoveHeight();

        transform.SetRotation(Quaternion.LookRotation(exitDir));

        yield return AccelerationMoveLogic(startPos, endPos, pitMoveHeight, pitSpeed);

        animator.SetBool("isFloating", false);

        transform.SetPosition(endPos);

        SetFloor(dirTile.floor);
        SetLockControl(false);

        moveCoroutine = null;
    }

    IEnumerator JumpMoveCoroutine(Vector3 targetPos)
    {
        VMCamera camera = GameManager.Instance.vmCamera;
        camera.SetCameraLerp(false);

        animator.SetBool("isFloating", true);

        SetLockControl(true);

        Vector3 startPos = transform.position;
        Vector3 endPos = GetPosUnit(targetPos);

        yield return AccelerationMoveLogic(startPos, endPos, jumpHeight, jumpSpeed);

        animator.SetBool("isFloating", false);

        camera.SetCameraLerp(true);

        transform.SetPosition(endPos);
        SetLockControl(false);

        moveCoroutine = null;
    }

    IEnumerator AccelerationMoveLogic(Vector3 startPos, Vector3 endPos, Vector3 curveHeight, float maxSpeed)
    {
        float speed = 0.1f;
        float t = 0f;

        while (t < 1f)
        {
            transform.BezierCurvePosition(startPos, endPos, curveHeight, t);

            t = Mathf.Lerp(t, 1.1f, speed * Time.smoothDeltaTime);
            speed = Mathf.Lerp(0.1f, maxSpeed, (t < 0.5f) ? (1f - t) : t);

            yield return null;
        }
    }
}
