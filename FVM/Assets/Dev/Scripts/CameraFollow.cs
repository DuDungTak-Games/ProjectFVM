using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{

    [SerializeField] Transform target;

    [SerializeField] Vector3 offsetPos;

    float followSpeed = 0.125f;

    void Update()
    {
        if (target == null)
            return;

        Vector3 targetPos = target.position + offsetPos;
        transform.position = Vector3.Lerp(transform.position, targetPos, followSpeed);
    }
}
