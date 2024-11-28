namespace Game.Inputs
{
public interface IAxisDetector
{
    float Axis { get; }
    float AxisNormalized { get; }
    bool HasAxisInput { get; }
}
}