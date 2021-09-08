using UnityEngine;

namespace DuDungTakGames.Extensions
{
    public static class VectorExtensions
    {
        public static Vector3 SetX(this ref Vector3 pos, float newX)
        {
            pos.x = newX;
            return pos;
        }
        
        public static Vector3 SetY(this ref Vector3 pos, float newY)
        {
            pos.y = newY;
            return pos;
        }
        
        public static Vector3 SetZ(this ref Vector3 pos, float newZ)
        {
            pos.z = newZ;
            return pos;
        }
        
        public static Vector3 Reset(this ref Vector3 pos)
        {
            pos = Vector3.zero;
            return pos;
        }
    }
}
