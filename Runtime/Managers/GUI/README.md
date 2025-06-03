# 🚀 GUI System Manager

A structured and extensible window management system.

## 📌 Features
- **Predefined Debug Panel** – Easily test and debug UI elements.
- **Window State** – Uses `index`, `mediator`, and `state` for better scalability.
- **Clash Royale UI Style** – new Window Manager().

## 📝 Code Snippet

### Window
```csharp
    internal enum $Window$WindowAction : byte
    {
        Unknown = 0,
        $END$
    }

    internal sealed class $Window$WindowButtonUI : Game.GUI.Windows.Components.BaseButton<$Window$WindowAction>
    {
        [ContextMenu("Editor simulate click")] protected override void SimulateClick() => base.SimulateClick();
        [ContextMenu("Editor force validate")] protected override void ForceValidate() => base.ForceValidate();
    }

    internal sealed class $Window$WindowViewUI : Game.GUI.Windows.WindowUI
    {
        public $Window$WindowButtonUI[] windowButtons;

#if UNITY_EDITOR
        [UnityEngine.ContextMenu("Collect window buttons")]
        private void CollectWindowButtons()
        {
            windowButtons = GetComponentsInChildren<$Window$WindowButtonUI>(true);

            UnityEditor.EditorUtility.SetDirty(this);
        }
#endif
    }

    internal sealed class $Window$WindowMediatorUI : BaseMediator<$Window$WindowViewUI>
    {
        public $Window$WindowMediatorUI($Window$WindowViewUI window) : base(window) { }
    }
```


### Window Reactive
```csharp
    internal enum $Window$WindowAction : byte
    {
        Unknown = 0,
        $END$
    }

    internal sealed class $Window$WindowButtonUI : Game.GUI.Windows.Components.BaseButton<$Window$WindowAction>
    {
        [ContextMenu("Editor simulate click")] protected override void SimulateClick() => base.SimulateClick();
        [ContextMenu("Editor force validate")] protected override void ForceValidate() => base.ForceValidate();
    }

    internal sealed class $Window$WindowViewUI : Game.GUI.Windows.WindowUI
    {
        public $Window$WindowButtonUI[] windowButtons;

#if UNITY_EDITOR
        [UnityEngine.ContextMenu("Collect window buttons")]
        private void CollectWindowButtons()
        {
            windowButtons = GetComponentsInChildren<$Window$WindowButtonUI>(true);

            UnityEditor.EditorUtility.SetDirty(this);
        }
#endif
    }

    internal sealed class $Window$WindowMediatorUI : BaseReactiveMediator<$Window$WindowViewUI, $Window$WindowAction>
    {
        public $Window$WindowMediatorUI($Window$WindowViewUI window)
            // ReSharper disable once CoVariantArrayConversion
            : base(window, window.windowButtons)
        {
        }

        public override void OnInitialize() => SubscribeToWindowsButtons();

        public override void OnDestroy() => UnsubscribeToWindowsButtons();

        protected override void ProceedButtonAction($Window$WindowAction action)
        {
            switch (action)
            {
                default:
                    throw new System.ArgumentOutOfRangeException(nameof(action), action, null);
            }
        }
    }
```