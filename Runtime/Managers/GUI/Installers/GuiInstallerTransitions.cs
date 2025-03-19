using Game.GUI.Windows.Transitions;
using UnityEngine;

namespace Game.GUI
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
    
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetStaticValues()
    {
        useCachedTransitions = false;
            
        _vertical = null;
        _verticalInverted = null;
        _horizontal = null;
        _horizontalInverted = null;
        _bounced = null;
        _empty = null;
        _fade = null;
    }
    
    public static IWindowTransition Vertical() => useCachedTransitions
        ? new VerticalTransition(_settings)
        : _vertical ??= new VerticalTransition(_settings);
    
    public static IWindowTransition VerticalInverted() => useCachedTransitions
        ? new VerticalInvertedTransition(_settings)
        : _verticalInverted ??= new VerticalInvertedTransition(_settings);
    
    public static IWindowTransition Horizontal() => useCachedTransitions
        ? new HorizontalTransition(_settings)
        : _horizontal ??= new HorizontalTransition(_settings);
    
    public static IWindowTransition HorizontalInverted() => useCachedTransitions
        ? new HorizontalInvertedTransition(_settings)
        : _horizontalInverted ??= new HorizontalInvertedTransition(_settings);
    
    public static IWindowTransition Bounced() => useCachedTransitions
        ? new BouncedTransition(_settings)
        : _bounced ??= new BouncedTransition(_settings);
    
    public static IWindowTransition Empty() => useCachedTransitions
        ? new EmptyTransition()
        : _empty ??= new EmptyTransition();
    
    public static IWindowTransition Fade() => useCachedTransitions
        ? new FadeTransition(_settings)
        : _fade ??= new FadeTransition(_settings);
    
    public static IWindowTransition Proxy(params IWindowTransition[] transitions) => new ParallelProxy(transitions);

    public static IWindowTransition Configurable(IWindowTransition open, IWindowTransition close,
                                                 bool openNormalize = true, bool closeNormalize = true)
        => new ConfigurableTransition(open, close, openNormalize, closeNormalize);
}
}