using UnityEngine;

public class GimicTrigger : GimicObject
{

    [TagSelector] 
    public string targetTag;

    protected GimicActor gimic;

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.CompareTag(targetTag))
        {
            OnTrigger();
        }
    }

    protected virtual void OnTrigger()
    {
        gimic.OnAction();
    }
}
