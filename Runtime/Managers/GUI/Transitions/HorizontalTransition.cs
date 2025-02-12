using System.Threading.Tasks;
using DG.Tweening;
using Game.GUI.Windows.Managers;
using Game.Extensions;
using UnityEngine;

namespace Game.GUI.Windows.Transitions
{
internal class HorizontalTransition : IWindowTransition
{
    private readonly WindowSettings _settings;
    private readonly int _width;
    private readonly int _height;

    public HorizontalTransition(WindowSettings settings)
    {
        _settings = settings;
        _width = Screen.width;
        _height = Screen.height;
    }

    public Task Open(WindowData windowData)
    {
        var completionSource = new TaskCompletionSource<bool>();

        var transform = windowData.Motor;
        var startedPos = GetStartedPointX(windowData.RectTransform);

        transform.SetLocalX(startedPos);
        MoveWindow(transform, 0, _settings.OpenType, () =>
        {
            completionSource.SetResult(true);
        });

        return completionSource.Task;
    }

    public Task Close(WindowData windowData)
    {
        var completionSource = new TaskCompletionSource<bool>();

        var transform = windowData.Motor;
        var targetPosition = GetEndPointX(windowData.RectTransform);

        MoveWindow(transform, targetPosition, _settings.CloseType, () =>
        {
            completionSource.SetResult(true);
        });

        return completionSource.Task;
    }

    private void MoveWindow(RectTransform transform, float to, Ease ease, TweenCallback completeAction)
    {
        transform.DOLocalMoveX(to, _settings.MoveDuration)
                 .SetEase(ease)
                 .OnComplete(completeAction);
    }

    protected virtual float GetStartedPointX(RectTransform transform)
    {
        var startingPoint = WindowTransitionStatic.startPoint;
        return startingPoint.x + transform.rect.width;
    }
    
    protected virtual float GetEndPointX(RectTransform transform)
    {
        var startingPoint = WindowTransitionStatic.startPoint;
        return startingPoint.x - transform.rect.width;
    }
}
}