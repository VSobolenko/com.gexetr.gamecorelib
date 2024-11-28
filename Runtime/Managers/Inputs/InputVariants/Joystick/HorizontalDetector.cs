using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Game.Inputs
{
public class HorizontalDetector : IAxisDetector, IDisposable
{
    private readonly InputEventHandler _eventHandler;
    private readonly RectTransform _lerpArea;

    private float Limit => _lerpArea.sizeDelta.x / 2;
    private Vector2 _start = Vector2.zero;
    private Vector2 _end = Vector2.zero;

    public float Axis => Mathf.Lerp(0, 1, (_end - _start).x / Limit) * Mathf.Sign((_end - _start).normalized.x);

    public float AxisNormalized => Mathf.Lerp(-1, 1, ((_end - _start).x / Limit) + 0.5f);
    //public float AxisNormalized => (_end - _start).normalized.x;
    public bool HasAxisInput => _end != _start;

    public HorizontalDetector(InputEventHandler eventHandler, RectTransform lerpArea)
    {
        _eventHandler = eventHandler;
        _lerpArea = lerpArea;
        SubscribeEvents();
    }

    private void OnPointerDown(PointerEventData eventData)
    {
        EventSystem.current.SetSelectedGameObject(_eventHandler.gameObject);
        _start = eventData.position;
        _end = _start;
    }

    private void OnDrag(PointerEventData eventData) => _end = eventData.position;

    private void OnPointerUp(PointerEventData eventData)
    {
        EventSystem.current.SetSelectedGameObject(null);
        _end = Vector2.zero;
        _start = _end;
    }

    private void SubscribeEvents()
    {
        _eventHandler.PointerDown += OnPointerDown;
        _eventHandler.PointerDrag += OnDrag;
        _eventHandler.PointerUp += OnPointerUp;
    }

    private void UnsubscribeEvents()
    {
        _eventHandler.PointerDown -= OnPointerDown;
        _eventHandler.PointerDrag -= OnDrag;
        _eventHandler.PointerUp -= OnPointerUp;
    }

    public void Dispose() => UnsubscribeEvents();
}
}