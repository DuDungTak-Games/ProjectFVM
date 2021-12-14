using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DuDungTakGames.Extensions;

public class GimicTrampoline : GimicActor
{
    
    [TagSelector] 
    public string targetTag;

    float targetFloor;

    Vector3 targetPos;

    Animator animator;

    protected override void Awake()
    {
        base.Awake();

        targetPos = Vector3.zero;

        animator = GetComponent<Animator>();
    }

    public override void OnAction()
    {
        if (targetPos != Vector3.zero)
        {
            PlayerController pc = GameManager.Instance.player.controller;
            pc.JumpMove(targetPos, targetFloor-1);

            animator.SetTrigger("OnTrigger");
        }
    }

    public override void OnActive()
    {
        FindTargetPos();
    }

    void FindTargetPos()
    {
        GimicManager gimicManager = GameManager.Instance.gimicManager;
        var gimicList = gimicManager.FindGimicCustom(this.ID);

        if(gimicList.Count > 0)
        {
            targetPos = gimicList[0].transform.position;
            targetFloor = gimicList[0].GetComponent<Tile>().floor;
        }
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.CompareTag(targetTag))
        {
            OnAction();
        }
    }
}
