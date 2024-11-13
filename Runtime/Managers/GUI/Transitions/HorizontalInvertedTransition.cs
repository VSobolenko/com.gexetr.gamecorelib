using UnityEngine;

namespace Game.GUI.Windows.Transitions
{
internal class HorizontalInvertedTransition : HorizontalTransition
{
    public HorizontalInvertedTransition(WindowSettings settings) : base(settings)
    {
    }

    protected override float GetStartedPointX(RectTransform transform)
    {
        var startingPoint = WindowTransitionStatic.startPoint;
        return startingPoint.x - transform.rect.width;
    }
    
    protected override float GetEndPointX(RectTransform transform)
    {
        var startingPoint = WindowTransitionStatic.startPoint;
        return startingPoint.x + transform.rect.width;
    }
}
}