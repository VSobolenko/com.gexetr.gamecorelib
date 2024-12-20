using System;
using UnityEngine;
using UnityEngine.UI;

namespace Game.GUI.Windows.Components
{
/// <summary>
/// for fast Editor Buttons add the following code:
/// 
/// [ContextMenu("Editor simulate click")] protected override void SimulateClick() => base.SimulateClick();
/// [ContextMenu("Editor force validate")] protected override void ForceValidate() => base.ForceValidate(); 
/// </summary>
/// <typeparam name="T"></typeparam>
public class BaseButton<T> : MonoBehaviour where T : struct, Enum
{
    [SerializeField] private ButtonConfiguration<T> configuration;
    
    public event Action<T> OnClickButton;

    private void Start()
    {
        configuration.ObserveButton();
        configuration.OnClickButton += ClickButton;
    }

    private void ClickButton(T action) => OnClickButton?.Invoke(action);
    
    private void OnValidate() => configuration?.ValidateButton(transform);
    
    // Cannot use as MenuItem, because log warning: .SimulateClick is generic and cannot be used for menu commands.
    protected virtual void SimulateClick() => configuration.SimulateClick();
    
    // Cannot use as MenuItem, because: .ForceValidate is generic and cannot be used for menu commands.
    protected virtual void ForceValidate() => OnValidate();
}

/// <summary>
/// To serialize generic parameters, it is necessary to use a nested-additional class!
/// </summary>
[Serializable,]
public class ButtonConfiguration<T> where T : struct, Enum
{
    [SerializeField] private Button button;
    [SerializeField] private T action;

    public event Action<T> OnClickButton;

    internal void ObserveButton() => button.onClick.AddListener(ButtonClick);
    
    private void ButtonClick() => OnClickButton?.Invoke(action);

    public void ValidateButton(Transform root)
    {
        if (button == null) button = root.GetComponent<Button>();
    }
    
    public void SimulateClick()
    {
        Log.Info($"Invoke editor simulation action: {action}");
        ButtonClick();
    }
}
}