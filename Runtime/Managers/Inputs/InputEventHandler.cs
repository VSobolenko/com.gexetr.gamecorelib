using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Game.Inputs
{
public class InputEventHandler : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    public event Action<PointerEventData> PointerDown;
    public event Action<PointerEventData> PointerDrag;
    public event Action<PointerEventData> PointerUp;

    public void OnPointerDown(PointerEventData eventData)
    { 
        PointerDown?.Invoke(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        PointerDrag?.Invoke(eventData);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        PointerUp?.Invoke(eventData);
    }
}
}