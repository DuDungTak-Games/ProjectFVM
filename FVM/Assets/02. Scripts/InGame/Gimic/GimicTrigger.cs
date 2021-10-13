using UnityEngine;
using UnityEngine.Events;

// NOTE : 이벤트가 여러 개라서 UnityEvent 를 사용함 (https://www.jacksondunstan.com/articles/3335)
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
}
