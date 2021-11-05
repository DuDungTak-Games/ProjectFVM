using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

using DuDungTakGames.Extensions;

public class PlayerController : MonoBehaviour
{
    
    public float moveSpeed = 14f;
    public float rotateSpeed = 14f;
    
    Coroutine moveCoroutine, rotateCoroutine;

    public void Move(Vector3 startPos, Vector3 movePos)
    {
        MoveCoroutine(startPos, movePos).Start(ref moveCoroutine, this);
    }

    public void Rotate(Vector3 direction)
    {
        RotateCoroutine(direction).Start(ref rotateCoroutine, this);
    }

    public bool isMoving()
    {
        return moveCoroutine != null;
    }
    
    public bool isRotating()
    {
        return rotateCoroutine != null;
    }

    IEnumerator MoveCoroutine(Vector3 startPos, Vector3 movePos)
    {
        yield return CoroutineExtensions.ProcessAction(moveSpeed, (t) =>
        {
            transform.BezierCurvePosition(startPos, movePos, (Vector3.up * 4f), t);
        });

        transform.SetPosition(movePos);

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

        rotateCoroutine = null;
    }
}
