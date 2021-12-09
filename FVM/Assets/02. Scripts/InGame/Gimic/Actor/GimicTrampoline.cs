using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DuDungTakGames.Extensions;

public class GimicTrampoline : GimicActor
{
    
    [TagSelector] 
    public string targetTag;

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
        if (targetPos == Vector3.zero)
            return;

        PlayerController pc = GameManager.Instance.player.controller;
        pc.JumpMove(targetPos);

        animator.SetTrigger("OnTrigger");
    }

    public override void CustomAction(GimicCustom gimic)
    {
        if(targetPos == Vector3.zero)
        {
            targetPos = gimic.transform.position;
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
