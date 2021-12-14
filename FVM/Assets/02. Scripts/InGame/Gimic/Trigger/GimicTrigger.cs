using UnityEngine;
using UnityEngine.Events;

public class GimicTrigger : GimicObject
{

    protected bool isTrigger;

    [SerializeField, TagSelector]
    protected string targetTag;

    [HideInInspector]
    public UnityEvent gimicEvent;

    public virtual void OnTrigger()
    {
        gimicEvent?.Invoke();
    }

    public void AddEvent(UnityAction action)
    {
        gimicEvent.AddListener(action);
    }
}
