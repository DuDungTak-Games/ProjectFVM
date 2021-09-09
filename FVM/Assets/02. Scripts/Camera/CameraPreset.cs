using System;
using System.Collections.Generic;
using UnityEngine;

using DuDungTakGames.CameraData;

[RequireComponent(typeof(VMCamera))]
public class CameraPreset : MonoBehaviour
{
    
    VMCamera vmCamera;

    [SerializeField] List<CameraData> datas = new List<CameraData>();

    void Awake()
    {
        vmCamera = this.GetComponent<VMCamera>();

        for (int i = 0; i < datas.Count; i++)
        {
            int index = i;
            TestGameManager.Instance.AddGameEvent(datas[index].onGameState, () =>
            {
                SetCameraData(index);
            });
        }
    }

    void SetCameraData(int index)
    {
        vmCamera.SetCameraPreset(datas[index]);
    }
}
