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

    Animator animator;

    protected override void Awake()
    {
        base.Awake();

        animator = GetComponent<Animator>();
    }

    public override void OnAction()
    {
        if(isOnceActive && isTrigger)
            return;
        
        isTrigger = !isTrigger;
        gameObject.layer = isTrigger ? (LayerMask.NameToLayer("Point")) : (LayerMask.NameToLayer("Tile"));
        
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
