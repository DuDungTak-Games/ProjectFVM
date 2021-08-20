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
