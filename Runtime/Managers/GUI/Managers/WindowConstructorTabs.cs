using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Game.Extensions;
using Game.GUI.Windows;
using UnityEngine;

namespace Game.GUI.Managers
{
public sealed class MediatorToTabBinder<TMediator, TTab> : Dictionary<TMediator, TTab>
    where TMediator : Type
    where TTab : struct, Enum { }

internal sealed class WindowConstructorTab<T, TSwitcher, TTabEnum> : IDisposable, IEnumerable<WindowData<T>>
    where T : class, IMediator
    where TSwitcher : ITabSwitcher<TTabEnum>
    where TTabEnum : struct, Enum
{
    private readonly MediatorToTabBinder<Type, TTabEnum> _tabBinder;
    private readonly TSwitcher _switcher;
    private readonly List<WindowData<T>> _windows = new(8);

    internal MediatorToTabBinder<Type, TTabEnum> TabBinder => _tabBinder;
    public TSwitcher Switcher => _switcher;

    private int _openedIndex = -1;

    public WindowConstructorTab(TSwitcher switcher, MediatorToTabBinder<Type, TTabEnum> tabBinder)
    {
        _switcher = switcher;
        _tabBinder = tabBinder;
    }

    public void InsertTab(T mediator, WindowUI window)
    {
        var windowData = new WindowData<T>
        {
            Mediator = mediator,
            RectTransform = (RectTransform) window.transform,
            CanvasGroup = window.config.canvasGroup,
        };
        windowData.Tab = window.config.overrideTabView != null ? window.config.overrideTabView : windowData.RectTransform;
        CorrectTabView(windowData.Tab);
        MoveUIWindow(windowData.RectTransform, AttachSide.Right, false);

        mediator.OnInitialize();

        _windows.Add(windowData);
        _switcher.SetAsLastSibling();
    }

    private void CorrectTabView(RectTransform windowDataTab)
    {
        var bottomOffset = _switcher.Height;
        var sizeDelta = windowDataTab.sizeDelta.SetY(-bottomOffset);
        var position = windowDataTab.anchoredPosition.AddY(sizeDelta.y / -2);
        windowDataTab.sizeDelta = sizeDelta;
        windowDataTab.anchoredPosition = position;

        if (bottomOffset > Screen.height / 2f)
            Log.Warner($"Switcher height({bottomOffset} takes up more than half the screen)");
    }

    public bool Has<TMediator>() where TMediator : class, IMediator =>
        _windows.Find(x => x.Mediator.GetType() == typeof(TMediator)) != null;

    public WindowData<T> OpenWindowSilently<TMediator>(Action<TMediator> initWindow = null)
        where TMediator : class, T
    {
        var mediatorType = typeof(TMediator);

        return InternalOpenWindow(mediatorType, mediator => { initWindow?.Invoke((TMediator) mediator); });
    }

    private WindowData<T> InternalOpenWindow(Type mediatorType, Action<T> initWindow = null)
    {
        int newOpenedIndex = -1;
        for (int i = 0; i < _windows.Count; i++)
        {
            if (_windows[i].Mediator.GetType() != mediatorType)
                continue;

            if (newOpenedIndex != -1)
                throw new IndexOutOfRangeException();

            newOpenedIndex = i;
        }

        var data = _windows[newOpenedIndex];

        if (_openedIndex == newOpenedIndex)
            return data;

        var moveSide = newOpenedIndex - _openedIndex > 0 ? AttachSide.Left : AttachSide.Right;
        var mediator = data.Mediator;
        mediator.SetInteraction(true);
        mediator.OnFocus();
        MoveUIWindow(data.RectTransform, moveSide, true);
        initWindow?.Invoke(mediator);

        if (newOpenedIndex - _openedIndex != 0 && _openedIndex >= 0)
        {
            var oldData = _windows[_openedIndex];
            var oldMediator = oldData.Mediator;
            oldMediator.SetInteraction(false);
            oldMediator.OnUnfocused();
            MoveUIWindow(oldData.RectTransform, moveSide, false);
        }

        var tabType = _tabBinder[_windows[newOpenedIndex].Mediator.GetType()];
        _switcher.OpenTabSmooth(tabType);
        _openedIndex = newOpenedIndex;

        return _windows[newOpenedIndex];
    }

    public void CloseWindow(int index)
    {
        if (_windows.ElementAtOrDefault(index) == null)
            return;

        var mediator = _windows[index].Mediator;
        mediator.OnUnfocused();
        mediator.OnDestroy();
        mediator.Destroy();

        _windows.RemoveAt(index);

        if (_windows.ElementAtOrDefault(index - 1) == null)
            return;

        var mediatorActivated = _windows[index - 1].Mediator;
        mediatorActivated.SetActive(true);
        mediatorActivated.SetInteraction(true);
        mediatorActivated.OnFocus();
        mediatorActivated.SetPosition(Vector3.zero);
    }
    
    public void Dispose() { }

    public WindowData<T> this[int i] => _windows[i];

    public WindowData<T> this[T i] => _windows.Find(x => x.Mediator == i);

    public WindowData<T> this[Type i] => _windows.Find(x => x.Mediator.GetType() == i);

    IEnumerator<WindowData<T>> IEnumerable<WindowData<T>>.GetEnumerator() => _windows.GetEnumerator();

    public IEnumerator GetEnumerator() => _windows.GetEnumerator();

    private void MoveUIWindow(RectTransform target, AttachSide side, bool isView)
    {
        if (isView)
        {
            target.anchoredPosition = target.anchoredPosition.SetX(0);

            return;
        }

        var position = target.anchoredPosition;

        position.x = side switch
        {
            AttachSide.Left => position.x - Screen.width,
            AttachSide.Right => position.x + Screen.width,
            _ => throw new ArgumentOutOfRangeException(nameof(side), side, null)
        };

        target.anchoredPosition = position;
    }

    private enum AttachSide : byte
    {
        Left,
        Right,
        Top,
        Bottom
    }
}
}