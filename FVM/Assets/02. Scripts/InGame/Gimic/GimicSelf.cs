using UnityEngine;

public class GimicSelf : GimicTrigger
{
    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.CompareTag(targetTag))
        {
            OnTrigger();
        }
    }
}