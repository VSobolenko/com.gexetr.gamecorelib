using System;

namespace Game.Inputs
{
public class StandaloneHorizontalAxisDetector : IAxisDetector
{
    public float Axis => throw new NotImplementedException();
    public float AxisNormalized => GetHorizontalAxis();
    public bool HasAxisInput => GetHorizontalAxis() != 0;

    public float GetHorizontalAxis() => UnityEngine.Input.GetAxis("Horizontal");
}

public class StandaloneVerticalAxisDetector : IAxisDetector
{
    public float Axis => throw new NotImplementedException();
    public float AxisNormalized => GetVerticalAxis();
    public bool HasAxisInput => GetVerticalAxis() != 0;

    public float GetVerticalAxis() => UnityEngine.Input.GetAxis("Vertical");
}
}