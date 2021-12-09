using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GimicDoor : GimicActor
{ 
    
    bool isTrigger;

    [SerializeField] 
    bool onStartActive;
    
    [SerializeField] 
    bool isOnceActive;

    Collider collider;
    
    Animator animator;

    protected override void Awake()
    {
        base.Awake();

        collider = GetComponent<Collider>();
        animator = GetComponent<Animator>();
    }

    public override void OnAction()
    {
        if(isOnceActive && isTrigger)
            return;
        
        isTrigger = !isTrigger;
        collider.enabled = !isTrigger;
        
        animator.SetBool("isTrigger", isTrigger);
    }
    
    public override void OnActive()
    {
        if (onStartActive)
        {
            OnAction();
        }
    }
}
