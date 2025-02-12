using System.Threading.Tasks;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using Game.GUI.Windows.Managers;
using UnityEngine;

namespace Game.GUI.Windows.Transitions
{
internal class FadeTransition : IWindowTransition
{
    private readonly WindowSettings _settings;

    public FadeTransition(WindowSettings settings)
    {
        _settings = settings;
    }

    public Task Open(WindowData windowData)
    {
        var completionSource = new TaskCompletionSource<bool>();
        var canvasGroup = windowData.CanvasGroup;

        canvasGroup.alpha = 0;

        FadeWindow(canvasGroup, 1, _settings.FadeDuration / 2f / _settings.Synchronicity, _settings.OpenType, null,
            () => { completionSource.SetResult(true); });

        return completionSource.Task;
    }

    public Task Close(WindowData windowData)
    {
        var completionSource = new TaskCompletionSource<bool>();
        var canvasGroup = windowData.CanvasGroup;

        FadeWindow(canvasGroup, 0, 0, _settings.OpenType, null,
            () => { completionSource.SetResult(true); });

        return completionSource.Task;
    }
    
    private void FadeWindow(CanvasGroup group, float to, float startDelay, Ease ease, TweenCallback actionAfterDelay, TweenCallback completeAction)
    {
        var seq = DOTween.Sequence();
        seq.PrependInterval(startDelay)
            .Append(DOFade(group, to, _settings.FadeDuration / 2f)
                .SetEase(ease)
                .OnStart(actionAfterDelay))
            .OnComplete(completeAction);
    }

    private static TweenerCore<float, float, FloatOptions> DOFade(CanvasGroup target, float endValue, float duration)
    {
        var t = DOTween.To(() => target.alpha, x => target.alpha = x, endValue, duration);
        t.SetTarget(target);
        return t;
    }
}
}