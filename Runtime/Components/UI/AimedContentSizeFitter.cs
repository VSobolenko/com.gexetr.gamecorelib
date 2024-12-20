using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Game.Components.UI
{
[AddComponentMenu("Layout/Aimed Content Size Fitter", 141)]
[ExecuteAlways]
[RequireComponent(typeof(RectTransform))]
public class AimedContentSizeFitter : UIBehaviour, ILayoutGroup
{
    public enum FitMode
    {
        Unconstrained,
        MinSize,
        PreferredSize
    }

    [SerializeField] private FitMode _horizontalFit = FitMode.Unconstrained;

    public FitMode horizontalFit
    {
        get => _horizontalFit;
        set
        {
            if (SetStruct(ref _horizontalFit, value)) SetDirty();
        }
    }

    [SerializeField] private FitMode _verticalFit = FitMode.Unconstrained;

    public FitMode verticalFit
    {
        get => _verticalFit;
        set
        {
            if (SetStruct(ref _verticalFit, value)) SetDirty();
        }
    }

    [SerializeField] private RectTransform _aimedRect;
    [NonSerialized] private RectTransform _selfRect;

    private RectTransform RectTransform
    {
        get
        {
            if (_selfRect == null)
                _selfRect = GetComponent<RectTransform>();
            return _selfRect;
        }
    }

    // field is never assigned warning
#pragma warning disable 649
    private DrivenRectTransformTracker _tracker;
#pragma warning restore 649

    protected override void OnEnable()
    {
        base.OnEnable();
        SetDirty();
    }

    protected override void OnDisable()
    {
        _tracker.Clear();
        LayoutRebuilder.MarkLayoutForRebuild(RectTransform);
        base.OnDisable();
    }

    protected override void OnRectTransformDimensionsChange()
    {
        SetDirty();
    }

    private void HandleSelfFittingAlongAxis(int axis)
    {
        if (_aimedRect == null)
            return;
        FitMode fitting = (axis == 0 ? horizontalFit : verticalFit);
        if (fitting == FitMode.Unconstrained)
        {
            _tracker.Add(this, RectTransform, DrivenTransformProperties.None);

            return;
        }

        _tracker.Add(this, RectTransform,
                     (axis == 0 ? DrivenTransformProperties.SizeDeltaX : DrivenTransformProperties.SizeDeltaY));
        if (fitting == FitMode.MinSize)
            RectTransform.SetSizeWithCurrentAnchors((RectTransform.Axis) axis,
                                                    LayoutUtility.GetMinSize(_aimedRect, axis));
        else
            RectTransform.SetSizeWithCurrentAnchors((RectTransform.Axis) axis,
                                                    LayoutUtility.GetPreferredSize(_aimedRect, axis));
    }

    public virtual void SetLayoutHorizontal()
    {
        _tracker.Clear();
        HandleSelfFittingAlongAxis(0);
    }

    public virtual void SetLayoutVertical()
    {
        HandleSelfFittingAlongAxis(1);
    }

    protected void SetDirty()
    {
        if (!IsActive())
            return;

        LayoutRebuilder.MarkLayoutForRebuild(RectTransform);
    }

    private static bool SetStruct<T>(ref T currentValue, T newValue) where T : struct
    {
        if (EqualityComparer<T>.Default.Equals(currentValue, newValue))
            return false;

        currentValue = newValue;

        return true;
    }

#if UNITY_EDITOR
    protected override void OnValidate()
    {
        SetDirty();
    }

#endif
}
}