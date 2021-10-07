using UnityEngine;

public class GimicTrigger : GimicObject
{

    protected GimicActor gimic;

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            OnTrigger();
        }
    }

    protected virtual void OnTrigger()
    {
        gimic.OnAction();
    }
}
