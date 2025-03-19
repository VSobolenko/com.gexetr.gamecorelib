# 🚀 GUI System Manager

A structured and extensible window management system.

## 📌 Features
- **Predefined Debug Panel** – Easily test and debug UI elements.
- **Window State** – Uses `index`, `mediator`, and `state` for better scalability.
- **Clash Royale UI Style** – new Window Manager().

## 📝 Code Snippet
```csharp
internal enum $Window$WindowAction : byte
{
    Unknown = 0,
    $END$
}

internal sealed class $Window$WindowButton : Game.GUI.Windows.Components.BaseButton<$Window$WindowAction>
{
    [ContextMenu("Editor simulate click")]
    protected override void SimulateClick() => base.SimulateClick();
    
    [ContextMenu("Editor force validate")]
    protected override void ForceValidate() => base.ForceValidate();
}

internal sealed class $Window$WindowView : WindowUI
{
    public $Window$WindowButton[] windowButtons;

    #if UNITY_EDITOR
    [ContextMenu("Collect window buttons")]
    private void CollectWindowButtons()
    {
        windowButtons = GetComponentsInChildren<$Window$WindowButton>(true);
        UnityEditor.EditorUtility.SetDirty(this);
    }
    #endif
}

internal sealed class $Window$WindowMediator : BaseReactiveMediator<$Window$WindowView, $Window$WindowAction>
{
    public $Window$WindowMediator($Window$WindowView window, BaseButton<$Window$WindowAction>[] reactiveButton)
        : base(window, reactiveButton)
    {
    }

    public override void OnInitialize() => SubscribeToWindowsButtons();
    public override void OnDestroy() => UnsubscribeToWindowsButtons();

    protected override void ProceedButtonAction($Window$WindowAction action) =>
        throw new System.NotImplementedException();
}
```