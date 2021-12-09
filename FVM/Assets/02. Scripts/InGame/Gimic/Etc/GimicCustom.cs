using UnityEngine;

public class GimicCustom : GimicObject
{

    protected override void Awake()
    {
        return;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        
        Gizmos.DrawWireCube(transform.position, Vector3.one * 0.2f);
    }
}
