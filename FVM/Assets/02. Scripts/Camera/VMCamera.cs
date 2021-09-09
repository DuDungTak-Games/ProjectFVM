using System.Collections;
using UnityEngine;

using DuDungTakGames.Extensions;

public class VMCamera : MonoBehaviour
{
    
    [Header("Camera")]
    [SerializeField] Vector3 offsetPos;

    [Header("Camera Follow")]
    [SerializeField] bool isLerp;
    
    [SerializeField] Transform target;
    
    [SerializeField] float followSpeed = 10f;
    
    Vector3 targetPos, resultPos, effectPos;

    // NOTE : Camera Shake
    float globalShakeAmount;
    
    Coroutine shakeCoroutine;

    void Update()
    {
        if (target != null)
        {
            targetPos = target.position;
        }

        resultPos = targetPos + offsetPos + effectPos;
        
        if (isLerp)
        {
            transform.position = Vector3.Lerp(transform.position, resultPos, followSpeed * Time.smoothDeltaTime);
        }
        else
        {
            transform.position = resultPos;
        }
    }
    
    public void ShakeLoop(float amount = 0.125f)
    {
        if (globalShakeAmount <= 0)
        {
            shakeCoroutine?.Stop(this);
            ShakeDone();
        }

        globalShakeAmount = amount;
        
        if (shakeCoroutine == null)
            ShakeCameraLoop().Start(ref shakeCoroutine, this);
    }

    public void Shake(float duration, float amount = 0.125f)
    {
        shakeCoroutine?.Stop(this);

        ShakeCamera(duration, amount).Start(ref shakeCoroutine, this);
    }

    IEnumerator ShakeCamera(float duration, float amount)
    {
        Vector3 originPos = effectPos;

        while (duration > 0)
        {
            yield return Shake(originPos, amount);
            duration -= Time.deltaTime;
        }

        ShakeDone();
    }
    
    IEnumerator ShakeCameraLoop()
    {
        Vector3 originPos = effectPos;

        while (globalShakeAmount > 0f)
        {
            yield return Shake(originPos, globalShakeAmount);
        }

        ShakeDone();
    }

    IEnumerator Shake(Vector3 originPos, float amount)
    {
        effectPos.Set(originPos + Random.insideUnitSphere * amount);
        yield return new WaitForEndOfFrame();
    }

    void ShakeDone()
    {
        effectPos.Reset();
        
        globalShakeAmount = 0f;
        shakeCoroutine = null;
    }
}
