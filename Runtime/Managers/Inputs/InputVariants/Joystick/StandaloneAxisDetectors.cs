using System;

namespace Game.Inputs
{
public sealed class StandaloneHorizontalAxisDetector : IAxisDetector
{
    public float Axis => throw new NotImplementedException();
    public float AxisNormalized => GetHorizontalAxis();
    public bool HasAxisInput => GetHorizontalAxis() != 0;

    public float GetHorizontalAxis() => UnityEngine.Input.GetAxis("Horizontal");
}

public sealed class StandaloneVerticalAxisDetector : IAxisDetector
{
    public float Axis => throw new NotImplementedException();
    public float AxisNormalized => GetVerticalAxis();
    public bool HasAxisInput => GetVerticalAxis() != 0;

    public float GetVerticalAxis() => UnityEngine.Input.GetAxis("Vertical");
}
}