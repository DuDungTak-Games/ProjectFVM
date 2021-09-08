using UnityEngine;

namespace DuDungTakGames.Extensions
{
    public static class TransformExtensions
    {
        public static void BezierCurvePosition(this Transform target, Vector3 startPos, Vector3 endPos, Vector3 heightOffset, float t)
        {
            Vector3 startPosH = startPos + heightOffset;
            Vector3 endPosH = endPos + heightOffset;
        
            Vector3 M0 = Vector3.Lerp(startPos, startPosH, t);
            Vector3 M1 = Vector3.Lerp(startPosH, endPosH, t);
            Vector3 M2 = Vector3.Lerp(endPosH, endPos, t);
        
            Vector3 B0 = Vector3.Lerp(M0, M1, t);
            Vector3 B1 = Vector3.Lerp(M1, M2, t);

            target.position = Vector3.Lerp(B0, B1, t);
        }

        public static void SetPosition(this Transform trf, Vector3 newPos)
        {
            trf.position = newPos;
        }

        public static void SetPositionX(this Transform trf, float newX)
        {
            Vector3 newPos = trf.position;
            trf.position = newPos.SetX(newX);
        }

        public static void SetPositionY(this Transform trf, float newY)
        {
            Vector3 newPos = trf.position;
            trf.position = newPos.SetY(newY);
        }
        
        public static void SetPositionZ(this Transform trf, float newZ)
        {
            Vector3 newPos = trf.position;
            trf.position = newPos.SetZ(newZ);
        }

        public static void SetRotation(this Transform trf, Quaternion newRot)
        {
            trf.rotation = newRot;
        }
    }
}
