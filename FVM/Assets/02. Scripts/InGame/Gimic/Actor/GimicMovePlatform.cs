using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DuDungTakGames.Extensions;

public class GimicMovePlatform : GimicActor
{
    
    [TagSelector] 
    public string targetTag;
    
    bool isTrigger, isTurnBack;

    int curIdx;
    
    [SerializeField] 
    bool onStartActive;
    
    [SerializeField] 
    bool isOnceActive;

    [SerializeField] 
    float moveDelay = 3;
    
    [SerializeField] 
    float moveSpeed = 1.25f;
    
    [SerializeField] 
    float moveInertia = 20f;
    
    List<Vector3> movePoints = new List<Vector3>();

    GameObject child;

    Coroutine loopCoroutine;

    protected override void Awake()
    {
        transform.rotation = Quaternion.identity;
    }

    public override void OnAction()
    {
        if(isOnceActive && isTrigger)
            return;
        
        isTrigger = !isTrigger;

        if (loopCoroutine == null)
        {
            LoopCoroutine().Start(ref loopCoroutine, this);
        }
    }

    public override void OnActive()
    {
        movePoints.Add(transform.position); // NOTE : START POINT

        FindMovePoints();

        curIdx = 1;
        
        if (onStartActive)
        {
            OnAction();
        }
    }

    void FindMovePoints()
    {
        GimicManager gimicManager = GameManager.Instance.gimicManager;
        var gimicList = gimicManager.FindGimicCustom(this.ID);

        foreach (var gimic in gimicList.OrderBy(x => x.ID))
        {
            movePoints.Add(gimic.transform.position);
            Destroy(gimic.gameObject);
        }
    }

    IEnumerator LoopCoroutine()
    {
        if (movePoints.Count < 2)
            yield break;

        bool isDelay = true;

        while (isTrigger)
        {
            if (isDelay)
            {
                isDelay = false;
                yield return new WaitForSeconds(moveDelay);
            }

            gameObject.layer = LayerMask.NameToLayer("Default");

            child?.transform.SetParent(this.transform);

            Vector3 startPoint = transform.position;
            Vector3 targetDir = (movePoints[curIdx] - startPoint).normalized * moveInertia;

            Quaternion startRot = Quaternion.Euler(targetDir.z * -1f, targetDir.y, targetDir.x);
            Quaternion endRot = Quaternion.identity;
            Quaternion targetRot = Quaternion.identity;

            yield return CoroutineExtensions.ProcessAction(moveSpeed, (t) =>
            {
                transform.position = Vector3.Lerp(startPoint, movePoints[curIdx], t);
                targetRot = Quaternion.Lerp(startRot, endRot, t);
                transform.rotation = Quaternion.Lerp(transform.rotation, targetRot, t);
            });

            transform.position = movePoints[curIdx];
            transform.rotation = Quaternion.identity;

            child?.transform.SetParent(null);

            if (NextPoint())
            {
                isDelay = true;
                gameObject.layer = LayerMask.NameToLayer("Tile");
            }
        }

        yield return null;
        
        loopCoroutine = null;
    }

    bool NextPoint()
    {
        bool isArrive = ((curIdx + 1) >= movePoints.Count) || ((curIdx - 1) <= -1);

        if(isArrive)
        {
            isTurnBack = ((curIdx + 1) >= movePoints.Count);
        }

        curIdx += isTurnBack ? -1 : 1;

        return isArrive;
    }

    void OnTriggerEnter(Collider col)
    {
        GameObject obj = col.gameObject;
        if (obj.CompareTag(targetTag))
        {
            child = obj;
        }
    }

    void OnTriggerExit(Collider col)
    {
        if (child == col.gameObject)
        {
            child.transform.SetParent(null);
            child = null;
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;

        if (movePoints.Count > 1)
        {
            Gizmos.DrawSphere(movePoints[0], 0.4f);
            
            for (int i = 1; i < movePoints.Count; i++)
            {
                Gizmos.DrawLine(movePoints[i-1], movePoints[i]);
                Gizmos.DrawSphere(movePoints[i], 0.2f);
            }
        }
    }
}
