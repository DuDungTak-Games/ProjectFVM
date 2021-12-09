using UnityEngine;

using DuDungTakGames.CameraData;
using DuDungTakGames.Extensions;

public class VMCamera : MonoBehaviour
{
    
    Camera mainCamera;

    [Header("Camera Info")]
    [SerializeField] Vector3 offsetPos;
    [SerializeField] Vector3 offsetRot;
    [SerializeField] float offsetZoom;

    [Header("Camera Follow")]
    [SerializeField] bool isLerp;
    
    [SerializeField] Transform target;
    
    [SerializeField] float followSpeed = 10f;
    
    Vector3 targetPos, resultPos, effectPos;

    void Awake()
    {
        mainCamera = Camera.main;

        Application.targetFrameRate = 60;
    }
    
    void Update()
    {
        UpdateCameraPos();
        UpdateCameraRot();
        UpdateCameraZoom();
    }

    void UpdateCameraPos()
    {
        if (target != null)
        {
            targetPos.Set(target.position);
        }

        resultPos.Set(targetPos + offsetPos + effectPos);
        
        if (isLerp)
        {
            transform.position = Vector3.Lerp(transform.position, resultPos, followSpeed * Time.smoothDeltaTime);
        }
        else
        {
            transform.SetPosition(resultPos);
        }
    }

    void UpdateCameraRot()
    {
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(offsetRot), followSpeed * Time.smoothDeltaTime);
    }
    
    void UpdateCameraZoom()
    {
        mainCamera.orthographicSize = Mathf.Lerp(mainCamera.orthographicSize, offsetZoom, followSpeed * Time.smoothDeltaTime);
    }

    public void SetCameraLerp(bool isOn)
    {
        isLerp = isOn;
    }

    public void SetCameraTarget(Transform trf)
    {
        target = trf;
    }
    
    public void SetCameraPreset(CameraData data)
    {
        offsetPos = data.offsetPos;
        offsetRot = data.offsetRot;
        offsetZoom = data.offsetZoom;
        followSpeed = data.followSpeed;
    }

    public Vector3 GetEffectPos()
    {
        return effectPos;
    }

    public void SetEffectPos(Vector3 newPos)
    {
        effectPos.Set(newPos);
    }
}
