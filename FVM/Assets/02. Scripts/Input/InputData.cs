using UnityEngine;
using UnityEngine.EventSystems;

namespace DuDungTakGames.Input
{
    public struct InputData
    {
        public Vector2 position { get; private set; }
        public Vector2 deltaPosition { get; private set; }

        public void SetData(Touch touch)
        {
            this.position = touch.position;
            this.deltaPosition = touch.deltaPosition;
        }

        public void SetData(Vector3 mousePosition)
        {
            this.position = mousePosition;
            this.deltaPosition = Vector3.zero;
        }

        public void SetData(PointerEventData eventData)
        {
            this.position = eventData.position;
            this.deltaPosition = eventData.delta;
        }
    }
}