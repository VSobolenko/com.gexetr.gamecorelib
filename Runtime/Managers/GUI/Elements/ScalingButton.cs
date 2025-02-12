using DG.Tweening;
using Game.Extensions;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Game.GUI
{
[DisallowMultipleComponent]
public class ScalingButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private float normalScale = 1f;
    [SerializeField] private float downScale = 0.96f;
    [SerializeField] private float upScale = 1.04f;
    [SerializeField] private bool resaveOnDown;

    public bool ResaveOnDown { get => resaveOnDown; set => resaveOnDown = value; }

    private Tween _scaleTween;

    public void OnPointerDown(PointerEventData eventData)
    {
        if (resaveOnDown)
            RewriteScaleValue().With(x => x.resaveOnDown = false);

        _scaleTween?.Kill();
        _scaleTween = transform.DOScale(downScale, 0.04f).SetEase(Ease.InOutCubic);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        _scaleTween?.Kill();
        _scaleTween = transform.DOScale(upScale, 0.08f).SetEase(Ease.InOutCubic).OnComplete(() =>
        {
            _scaleTween?.Kill();
            _scaleTween = transform.DOScale(normalScale, 0.06f).SetEase(Ease.InOutCubic);
        });
    }

    private ScalingButton RewriteScaleValue()
    {
        var scale = transform.localScale;
        var downDelta = normalScale - downScale;
        var upDelta = upScale - normalScale;

        normalScale = scale.magnitude;
        downScale = normalScale - downDelta;
        upScale = normalScale + upDelta;
        return this;
    }
    
    private void OnDestroy() => _scaleTween?.Kill();
}
}