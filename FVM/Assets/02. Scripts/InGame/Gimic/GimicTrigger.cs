using System;
using UnityEngine;
using UnityEngine.Events;

public class GimicTrigger : GimicObject
{

    [TagSelector] 
    public string targetTag;

    [HideInInspector]
    public UnityEvent gimicEvent;

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.CompareTag(targetTag))
        {
            OnTrigger();
        }
    }

    protected virtual void OnTrigger()
    {
        gimicEvent?.Invoke();
    }

    public void AddEvent(UnityAction action)
    {
        gimicEvent.AddListener(action);
    }
}
