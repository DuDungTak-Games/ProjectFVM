using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GimicLever : GimicTrigger
{
    
    bool isTrigger;
    
    Animator animator;
    
    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public override void OnTrigger()
    {
        isTrigger = !isTrigger;
        
        animator.SetBool("isTrigger", isTrigger);
        animator.SetTrigger("OnTrigger");
        
        base.OnTrigger();
    }
}
