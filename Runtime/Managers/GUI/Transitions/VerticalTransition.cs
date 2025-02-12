using System.Threading.Tasks;
using DG.Tweening;
using Game.GUI.Windows.Managers;
using Game.Extensions;
using UnityEngine;

namespace Game.GUI.Windows.Transitions
{
internal class VerticalTransition : IWindowTransition
{
    private readonly WindowSettings _settings;
    private readonly int _width;
    private readonly int _height;

    public VerticalTransition(WindowSettings settings)
    {
        _settings = settings;
        _width = Screen.width;
        _height = Screen.height;
    }

    public Task Open(WindowData windowData)
    {
        var completionSource = new TaskCompletionSource<bool>();

        var transform = windowData.Motor;
        var startPosY = GetStartedPointY(windowData.RectTransform);

        transform.SetLocalY(startPosY);
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
        var targetPositionY = GetEndPointY(windowData.RectTransform);
        
        MoveWindow(transform, targetPositionY, _settings.CloseType, () =>
        {
            completionSource.SetResult(true);
        });

        return completionSource.Task;
    }

    private void MoveWindow(RectTransform transform, float to, Ease ease, TweenCallback completeAction)
    {
        transform.DOLocalMoveY(to, _settings.MoveDuration)
                 .SetEase(ease)
                 .OnComplete(completeAction);
    }
    
    protected virtual float GetStartedPointY(RectTransform transform)
    {
        var startingPoint = WindowTransitionStatic.startPoint;
        return startingPoint.y + transform.rect.height;
    }
    
    protected virtual float GetEndPointY(RectTransform transform)
    {
        var startingPoint = WindowTransitionStatic.startPoint;
        return startingPoint.y - transform.rect.height;
    }
}
}