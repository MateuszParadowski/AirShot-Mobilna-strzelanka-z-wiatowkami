using UnityEngine;
using UnityEngine.EventSystems;

public class LookArea : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
{
    public Vector2 lookDelta;

    public void OnPointerDown(PointerEventData eventData) { }

    public void OnPointerUp(PointerEventData eventData)
    {
        lookDelta = Vector2.zero;
    }

    public void OnDrag(PointerEventData eventData)
    {
        lookDelta = eventData.delta;
    }
}
