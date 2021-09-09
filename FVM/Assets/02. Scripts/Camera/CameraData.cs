using System;
using UnityEngine;

namespace DuDungTakGames.CameraData
{
    [Serializable]
    public struct CameraData
    {
        public gameState onGameState;
        public Vector3 offsetPos;
        public Vector3 offsetRot;
        public float offsetZoom;
        public float followSpeed;
    }
}

