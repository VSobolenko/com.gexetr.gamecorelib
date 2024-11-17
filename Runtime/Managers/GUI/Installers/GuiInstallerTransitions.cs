using System;
using Game.GUI.Windows;
using Game.GUI.Windows.Transitions;

namespace Game.GUI.Installers
{
public static partial class GuiInstaller
{
    public static bool useCachedTransitions = false;

    private static IWindowTransition _vertical;
    private static IWindowTransition _verticalInverted;
    private static IWindowTransition _horizontal;
    private static IWindowTransition _horizontalInverted;
    private static IWindowTransition _bounced;
    private static IWindowTransition _empty;
    private static IWindowTransition _fade;
    
    public static IWindowTransition Vertical() => useCachedTransitions
        ? new VerticalTransition(_windowSettings)
        : _vertical ??= new VerticalTransition(_windowSettings);
    
    public static IWindowTransition VerticalInverted() => useCachedTransitions
        ? new VerticalInvertedTransition(_windowSettings)
        : _verticalInverted ??= new VerticalInvertedTransition(_windowSettings);
    
    public static IWindowTransition Horizontal() => useCachedTransitions
        ? new HorizontalTransition(_windowSettings)
        : _horizontal ??= new HorizontalTransition(_windowSettings);
    
    public static IWindowTransition HorizontalInverted() => useCachedTransitions
        ? new HorizontalInvertedTransition(_windowSettings)
        : _horizontalInverted ??= new HorizontalInvertedTransition(_windowSettings);
    public static IWindowTransition Bounced() => useCachedTransitions
        ? new BouncedTransition(_windowSettings)
        : _bounced ??= new BouncedTransition(_windowSettings);
    public static IWindowTransition Empty() => useCachedTransitions
        ? new EmptyTransition()
        : _empty ??= new EmptyTransition();
    public static IWindowTransition Fade() => useCachedTransitions
        ? new FadeTransition(_windowSettings)
        : _fade ??= new FadeTransition(_windowSettings);
    
    public static IWindowTransition Proxy(params IWindowTransition[] transitions) => new ParallelProxy(transitions);

    public static IWindowTransition Configurable(IWindowTransition open, IWindowTransition close,
                                                 bool openNormalize = true, bool closeNormalize = true)
        => new ConfigurableTransition(open, close, openNormalize, closeNormalize);
}
}