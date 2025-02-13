using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Game.Inputs
{
public class VerticalDetector : IAxisDetector, IDisposable
{
    private readonly InputEventHandler _eventHandler;
    private readonly Func<Vector2> _getLimit;

    private float Limit => _getLimit().y / 2;
    private Vector2 _start;
    private Vector2 _end;

    public float Axis => Mathf.Lerp(-1, 1, (_end - _start).y / Limit);
    public float AxisNormalized => Mathf.Lerp(-1, 1, (_end - _start).normalized.y / Limit);
    public bool HasAxisInput => _end != _start;

    public VerticalDetector(InputEventHandler eventHandler, Func<Vector2> getLerpArea)
    {
        _eventHandler = eventHandler;
        _getLimit = getLerpArea;
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
        _end = eventData.position;
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