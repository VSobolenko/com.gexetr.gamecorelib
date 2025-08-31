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

    private PointerEventData _eventData;

    public virtual void OnPointerDown(PointerEventData eventData)
    {
        _eventData = eventData;
        PointerDown?.Invoke(eventData);
    }

    public virtual void OnDrag(PointerEventData eventData)
    {
        _eventData = eventData;
        PointerDrag?.Invoke(eventData);
    }

    public virtual void OnPointerUp(PointerEventData eventData)
    {
        _eventData = null;
        PointerUp?.Invoke(eventData);
    }

    public bool TryTakeOverControl<T>(T to)
        where T : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler =>
        TryTakeOverControl(to, _eventData);

    public bool TryTakeOverControl<T>(T to, PointerEventData eventData)
        where T : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
    {
        if (eventData == null)
            return false;

        eventData.pointerPress = to.gameObject;
        eventData.pointerDrag = to.gameObject;

        return true;
    }
}
}