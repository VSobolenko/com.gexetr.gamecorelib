﻿using System.Threading.Tasks;
using DG.Tweening;
using Game.GUI.Windows.Managers;
using UnityEngine;

namespace Game.GUI.Windows.Transitions
{
internal class BouncedTransition : IWindowTransition
{
    private readonly Vector3 _openStartedScale = new(0.3f, 0.3f, 0.3f);
    private readonly Vector3 _closeEndScale = new(0.3f, 0.3f, 0.3f);
    private readonly WindowSettings _settings;
    private readonly int _width;
    private readonly int _height;

    public BouncedTransition(WindowSettings settings)
    {
        _settings = settings;
        _width = Screen.width;
        _height = Screen.height;
    }

    public Task Open(WindowProperties windowProperties)
    {
        var completionSource = new TaskCompletionSource<bool>();
        var transform = windowProperties.rectTransform;
        
        windowProperties.mediator.SetActive(false);
        transform.localScale = _openStartedScale;

        BounceWindow(transform, Vector3.one, _settings.MoveDuration / 2f / _settings.Synchronicity, _settings.OpenType,
            () =>
            {
                windowProperties.mediator.SetActive(true);
            },
            () =>
            {
                completionSource.SetResult(true);
            });

        return completionSource.Task;
    }

    public Task Close(WindowProperties windowProperties)
    {
        var completionSource = new TaskCompletionSource<bool>();
        var transform = windowProperties.rectTransform;

        BounceWindow(transform, _closeEndScale, 0, _settings.CloseType, null, () =>
        {
            windowProperties.mediator.SetActive(false);
            completionSource.SetResult(true);
        });

        return completionSource.Task;
    }

    private void BounceWindow(Transform transform, Vector3 to, float startDelay, Ease ease, TweenCallback actionAfterDelay, TweenCallback completeAction)
    {
        var seq = DOTween.Sequence();
        seq.PrependInterval(startDelay)
           .Append(transform.DOScale(to, _settings.MoveDuration / 2f)
                            .SetEase(ease)
                            .OnStart(actionAfterDelay))
           .OnComplete(completeAction);
    }
}
}