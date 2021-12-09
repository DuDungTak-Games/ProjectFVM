using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GimicLever : GimicTrigger
{

    [SerializeField]
    bool isOnceTrigger;
    
    Animator animator;
    
    protected override void Awake()
    {
        base.Awake();
        
        animator = GetComponent<Animator>();
    }

    public override void OnTrigger()
    {
        if (isOnceTrigger && isTrigger)
            return;
        
        isTrigger = !isTrigger;
        
        animator.SetBool("isTrigger", isTrigger);
        animator.SetTrigger("OnTrigger");
        
        base.OnTrigger();
    }
}
