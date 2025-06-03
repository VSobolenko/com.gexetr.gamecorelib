using Game.GUI.Windows.Components;
using UnityEngine;

namespace Game.GUI.Windows
{
public abstract class BaseReactiveMediator<TWindow, TReactive> : BaseMediator<TWindow> 
    where TWindow : WindowUI 
    where TReactive : struct, System.Enum
{
    protected readonly BaseButton<TReactive>[] reactiveButton;

    protected BaseReactiveMediator(TWindow window, BaseButton<TReactive>[] reactiveButton) : base(window)
    {
        this.reactiveButton = reactiveButton;
    }

    protected void SubscribeToWindowsButtons()
    {
        foreach (var button in reactiveButton)
            button.OnClickButton += ProceedButtonAction;
    }

    protected void UnsubscribeToWindowsButtons()
    {
        foreach (var button in reactiveButton)
            button.OnClickButton -= ProceedButtonAction;
    }

    protected virtual void ProceedButtonAction(TReactive action) { }
}
}