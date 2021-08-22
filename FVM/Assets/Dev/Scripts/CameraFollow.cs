using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{

    [SerializeField] bool isLerp;

    [SerializeField] Transform target;

    [SerializeField] Vector3 offsetPos;

    Vector3 targetPos = Vector3.zero;
    
    float followSpeed = 0.125f;

    void Update()
    {
        if (target != null)
        {
            targetPos = target.position;
            
            transform.LookAt(target);
        }

        Vector3 curPos = targetPos + offsetPos;

        if(isLerp)
        {
            transform.position = Vector3.Lerp(transform.position, curPos, followSpeed);
        }
        else
        {
            transform.position = curPos;
        }
    }
    
    
    
    // NOTE : ½ÇÇè¿ë
    public void RotateOffset(bool isRight)
    {
        int x = (int)Mathf.Sign(offsetPos.x);
        int z = (int)Mathf.Sign(offsetPos.z);

        if (x == z)
        {
            offsetPos.x = isRight ? offsetPos.x * -1 : offsetPos.x;
            offsetPos.z = isRight ? offsetPos.z : offsetPos.z * -1;
        }

        if (x != z)
        {
            offsetPos.x = isRight ? offsetPos.x : offsetPos.x * -1;
            offsetPos.z = isRight ? offsetPos.z * -1 : offsetPos.z;
        }
    }

    public void Shake(float duration, float amount = 0.3f)
    {
        StartCoroutine(ShakeCamera(duration, amount));
    }

    IEnumerator ShakeCamera(float duration, float amount = 0.3f)
    {
        Vector3 backupPos = offsetPos;

        while (duration > 0)
        {
            offsetPos = backupPos + Random.insideUnitSphere * amount;
            duration -= Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        offsetPos = backupPos;
    }
}
