using Game.GUI.Windows.Transitions;

namespace Game.GUI.Installers
{
public static partial class GuiInstaller
{
    public static IWindowTransition Vertical() => new VerticalTransition(WindowSettings);
    public static IWindowTransition VerticalInverted() => new VerticalInvertedTransition(WindowSettings);
    public static IWindowTransition Horizontal() => new HorizontalTransition(WindowSettings);
    public static IWindowTransition HorizontalInverted() => new HorizontalInvertedTransition(WindowSettings);
    public static IWindowTransition Bounced() => new BouncedTransition(WindowSettings);
    public static IWindowTransition Empty() => new EmptyTransition();
    public static IWindowTransition Fade() => new FadeTransition(WindowSettings);
    public static IWindowTransition Proxy(params IWindowTransition[] transitions) => new ParallelProxy(transitions);
}
}