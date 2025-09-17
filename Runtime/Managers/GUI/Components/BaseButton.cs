using System;
using Game.Exceptions;
using Game.Extensions;
using UnityEngine;
using UnityEngine.UI;

namespace Game.GUI.Components
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
    [SerializeField] protected ButtonConfiguration<T> configuration;
    
    public event Action<T> OnClickButton;

    private void Start()
    {
#if UNITY_EDITOR
        if (configuration.Button == null)
            throw new GCLInspectorReferenceException(this, nameof(configuration.Button));
#endif

        configuration.Observe();
        configuration.OnClickButton += ClickButton;
    }

    private void OnDestroy()
    {
#if UNITY_EDITOR
        if (configuration.Button == null)
            throw new GCLInspectorReferenceException(this, nameof(configuration.Button));
#endif

        configuration.Forget();
        configuration.OnClickButton -= ClickButton;
    }
    
    private void ClickButton(T action) => OnClickButton?.Invoke(action);

    protected virtual void Reset()
    {
        if (configuration == null)
            configuration = new ButtonConfiguration<T>();

        configuration.Validate(transform);
    }

    // Cannot use as MenuItem, because log warning: .SimulateClick is generic and cannot be used for menu commands.
    protected virtual void SimulateClick() => configuration.SimulateClick();
    
    // Cannot use as MenuItem, because: .ForceValidate is generic and cannot be used for menu commands.
    protected virtual void ForceValidate() => configuration.Validate(transform);
}

/// <summary>
/// To serialize generic parameters, it is necessary to use a nested-additional class!
/// </summary>
[Serializable]
public sealed class ButtonConfiguration<T> where T : struct, Enum
{
    [SerializeField] private Button _button;
    [SerializeField] private T _action;

    public T ActionType => _action;
    
    public Button Button => _button;
    
    public event Action<T> OnClickButton;

    internal void Observe() => _button.onClick.AddListener(ButtonClick);
    
    internal void Forget() => _button.onClick.RemoveListener(ButtonClick);
    
    private void ButtonClick() => OnClickButton?.Invoke(_action);

    public void Validate(Transform root) => this.With(x => x._button = root.GetComponent<Button>(), _button == null);

    public void SimulateClick()
    {
        Log.Info($"Invoke editor simulation button action: {_action}");
        ButtonClick();
    }
}
}