using System;
using UnityEngine;

public class GimicPressure : GimicTrigger
{

    [SerializeField] 
    bool isKeepPressure;

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
        base.OnTrigger();

        if (isOnceTrigger)
        {
            gimicEvent.RemoveAllListeners();
        }
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.CompareTag(targetTag) || col.gameObject.CompareTag("Tile"))
        {
            animator.SetBool("isTrigger", true);

            OnTrigger();
        }
    }

    void OnTriggerExit(Collider col)
    {
        if (col.gameObject.CompareTag(targetTag) || col.gameObject.CompareTag("Tile"))
        {
            animator.SetBool("isTrigger", isOnceTrigger);

            if (isKeepPressure)
            {
                OnTrigger();
            }
        }
    }
}
