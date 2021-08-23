using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{

    [SerializeField] bool isLerp, isLookAt;

    [SerializeField] Transform target;

    [SerializeField] Vector3 offsetPos;

    Vector3 targetPos = Vector3.zero;
    
    float followSpeed = 0.125f;
    
    float shakeAmount;

    Coroutine shakeCoroutine;

    void Update()
    {
        if (target != null)
        {
            targetPos = target.position;

            if (isLookAt)
            {
                transform.LookAt(target);
            }
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

    public void ShakeLoop(float amount)
    {
        shakeAmount = amount;

        if (shakeCoroutine == null)
        {
            shakeCoroutine = StartCoroutine(ShakeCamera());
        }
    }

    public void Shake(float duration, float amount = 0.3f)
    {
        if (shakeCoroutine != null)
        {
            StopCoroutine(shakeCoroutine);
        }

        shakeAmount = amount;
        shakeCoroutine = StartCoroutine(ShakeCamera(duration));
    }

    IEnumerator ShakeCamera(float duration)
    {
        Vector3 backupPos = offsetPos;

        while (duration > 0)
        {
            offsetPos = backupPos + Random.insideUnitSphere * shakeAmount;
            duration -= Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        offsetPos = backupPos;

        shakeCoroutine = null;
    }

    IEnumerator ShakeCamera()
    {
        Vector3 backupPos = offsetPos;
        
        while (shakeAmount > 0)
        {
            offsetPos = backupPos + Random.insideUnitSphere * shakeAmount;
            yield return new WaitForEndOfFrame();
        }
        
        offsetPos = backupPos;

        shakeCoroutine = null;
    }
}
