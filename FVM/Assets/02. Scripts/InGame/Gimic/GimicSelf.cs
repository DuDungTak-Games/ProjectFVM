using UnityEngine;

public class GimicSelf : GimicObject
{

    [TagSelector] 
    public string targetTag;

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.CompareTag(targetTag))
        {
            OnTrigger();
        }
    }

    protected virtual void OnTrigger()
    {
        
    }
}