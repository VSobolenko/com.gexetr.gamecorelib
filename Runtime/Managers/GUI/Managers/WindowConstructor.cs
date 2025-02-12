﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Game.GUI.Windows.Factories;
using UnityEngine;

namespace Game.GUI.Windows.Managers
{
internal class WindowConstructor : IDisposable, IEnumerable<WindowData>
{
    public int Count => _windows.Count;

    private readonly List<WindowData> _windows = new(8);
    private readonly IWindowFactory _windowFactory;
    private readonly Transform _root;

    public WindowConstructor(IWindowFactory windowFactory, Transform root)
    {
        _windowFactory = windowFactory;
        _root = root;
    }

    public void Dispose()
    {
        for (var i = _windows.Count - 1; i >= 0; i--)
            CloseWindow(i);

        _windows.Clear();
    }

    public WindowData OpenWindowSilently<TMediator>(Action<TMediator> initWindow = null)
        where TMediator : class, IMediator
    {
        if (_windowFactory.TryCreateWindow<TMediator>(_root, out var mediator, out var window) == false)
            throw new ArgumentNullException(typeof(TMediator).Name, $"Can't create mediator {typeof(TMediator)}");

        mediator.SetActive(true);
        mediator.SetInteraction(true);
        mediator.OnInitialize();
        mediator.OnFocus();
        initWindow?.Invoke(mediator);

        var windowData = new WindowData
        {
            Mediator = mediator,
            RectTransform = window.overrideTransition != null ? window.overrideTransition : (RectTransform) window.transform,
            CanvasGroup = window.canvasGroup,
            Mode = OpenMode.Silently,
        };
        windowData.Motor = window.overrideTransition != null ? window.overrideTransition : windowData.RectTransform;
        
        var lastWindowData = _windows.Count > 0 ? _windows[^1] : windowData;
        _windows.Add(windowData);

        if (lastWindowData != windowData)
        {
            lastWindowData.Mediator.OnUnfocused();
            lastWindowData.Mediator.SetInteraction(false);
            lastWindowData.Mediator.SetActive(windowData.Mode == OpenMode.Silently);
        }

        return windowData;
    }

    public void CloseWindow(int index)
    {
        if (_windows.ElementAtOrDefault(index) == null)
            return;

        _windows[index].Mediator.OnUnfocused();
        _windows[index].Mediator.OnDestroy();
        _windows[index].Mediator.Destroy();

        _windows.RemoveAt(index);

        if (_windows.ElementAtOrDefault(index - 1) == null)
            return;

        //Имеется общее
        _windows[index - 1].Mediator.SetActive(true);
        _windows[index - 1].Mediator.SetInteraction(true);
        _windows[index - 1].Mediator.OnFocus();
        _windows[index - 1].Mediator.SetPosition(Vector3.zero);
        RestoreVisibilityMode();
    }

    private void RestoreVisibilityMode()
    {
        if (_windows.Count == 1)
            return;
        
        for (var i = _windows.Count - 2; i >= 0; i--)
        {
            var parentWindowData = _windows[i + 1];

            _windows[i].Mediator.SetActive(parentWindowData.Mode == OpenMode.Silently);
            if (parentWindowData.Mode == OpenMode.Overlay)
                break;
        }
    }
    
    public void HideWindow(int index, bool deactivateLastWindow)
    {
        if (_windows.ElementAtOrDefault(index)?.Mediator == null)
            return;

        _windows[index].Mediator.OnUnfocused();
        if (deactivateLastWindow)
            _windows[index].Mediator.SetActive(false);
    }

    public WindowData this[int i] => _windows[i];

    IEnumerator<WindowData> IEnumerable<WindowData>.GetEnumerator() => _windows.GetEnumerator();

    public IEnumerator GetEnumerator() => _windows.GetEnumerator();
}
}