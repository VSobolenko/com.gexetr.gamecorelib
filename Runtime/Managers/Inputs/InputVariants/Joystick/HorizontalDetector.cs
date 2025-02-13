using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Game.Inputs
{
public class HorizontalDetector : IAxisDetector, IDisposable
{
    private readonly InputEventHandler _eventHandler;
    private readonly Func<Vector2> _getLimit;

    private Vector2 _start = Vector2.zero;
    private Vector2 _end = Vector2.zero;

    //public float Axis => Mathf.Lerp(0, 1, (_end - _start).x / Limit) * Mathf.Sign((_end - _start).normalized.x);
    public float Axis => throw new NotImplementedException();
    public float AxisNormalized => Mathf.Lerp(-1, 1, ((_end - _start).x / _getLimit.Invoke().x) + 0.5f);

    public bool HasAxisInput => _end != _start;

    public HorizontalDetector(InputEventHandler eventHandler, Func<Vector2> getLerpArea)
    {
        _eventHandler = eventHandler;
        _getLimit = getLerpArea;
        SubscribeEvents();
    }

    public HorizontalDetector Intercept(PointerEventData pointerDown)
    {
        OnPointerDown(pointerDown);

        return this;
    }

    private void OnPointerDown(PointerEventData eventData)
    {
        _start = eventData.position;
        _end = _start;
    }

    private void OnDrag(PointerEventData eventData) => _end = eventData.position;

    private void OnPointerUp(PointerEventData eventData)
    {
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