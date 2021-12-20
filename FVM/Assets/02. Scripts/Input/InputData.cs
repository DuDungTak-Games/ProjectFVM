using UnityEngine;
using UnityEngine.EventSystems;

namespace DuDungTakGames.Input
{
    public class InputData
    {
        public Vector2 position { get; private set; }
        public Vector2 deltaPosition { get; private set; }

        public InputData SetData(Touch touch)
        {
            this.position = touch.position;
            this.deltaPosition = touch.deltaPosition;

            return this;
        }

        public InputData SetData(Vector3 mousePosition)
        {
            this.position = mousePosition;
            this.deltaPosition = Vector3.zero;

            return this;
        }

        public InputData SetData(PointerEventData eventData)
        {
            this.position = eventData.position;
            this.deltaPosition = eventData.delta;

            return this;
        }
    }
}