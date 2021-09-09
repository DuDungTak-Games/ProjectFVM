using System.Collections;
using UnityEngine;

using DuDungTakGames.Extensions;

[RequireComponent(typeof(VMCamera))]
public class CameraShake : MonoBehaviour
{
    VMCamera vmCamera;
    
    float globalShakeAmount;
    
    Coroutine shakeCoroutine;

    void Awake()
    {
        vmCamera = GetComponent<VMCamera>();
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
        Vector3 originPos = vmCamera.GetEffectPos();

        while (duration > 0)
        {
            yield return Shake(originPos, amount);
            duration -= Time.deltaTime;
        }

        ShakeDone();
    }
    
    IEnumerator ShakeCameraLoop()
    {
        Vector3 originPos = vmCamera.GetEffectPos();

        while (globalShakeAmount > 0f)
        {
            yield return Shake(originPos, globalShakeAmount);
        }

        ShakeDone();
    }

    IEnumerator Shake(Vector3 originPos, float amount)
    {
        vmCamera.SetEffectPos(originPos + Random.insideUnitSphere * amount);
        yield return new WaitForEndOfFrame();
    }

    void ShakeDone()
    {
        vmCamera.SetEffectPos(Vector3.zero);
        
        globalShakeAmount = 0f;
        shakeCoroutine = null;
    }
}
