using Game.GUI.Components;
using UnityEngine;

namespace Game.GUI.Transitions
{
internal sealed class VerticalInvertedTransition : VerticalTransition
{
    public VerticalInvertedTransition(WindowSettings settings) : base(settings) { }

    protected override float GetStartedPointY(RectTransform transform)
    {
        var startingPoint = WindowTransitionStatic.startPoint;

        return startingPoint.y - transform.rect.height;
    }

    protected override float GetEndPointY(RectTransform transform)
    {
        var startingPoint = WindowTransitionStatic.startPoint;

        return startingPoint.y + transform.rect.height;
    }
}
}