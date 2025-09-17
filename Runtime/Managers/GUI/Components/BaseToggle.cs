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
public class BaseToggle<T> : MonoBehaviour where T : struct, Enum
{
    [SerializeField] protected ToggleConfiguration<T> configuration;

    public event Action<T, bool> OnClickToggle;

    private void Start()
    {
#if UNITY_EDITOR
        if (configuration.Toggle == null)
            throw new GCLInspectorReferenceException(this, nameof(configuration.Toggle));
#endif

        configuration.Observe();
        configuration.OnClickToggle += ClickToggle;
    }

    private void OnDestroy()
    {
#if UNITY_EDITOR
        if (configuration.Toggle == null)
            throw new GCLInspectorReferenceException(this, nameof(configuration.Toggle));
#endif
        
        configuration.Forget();
        configuration.OnClickToggle -= ClickToggle;
    }
    
    private void ClickToggle(T action, bool value) => OnClickToggle?.Invoke(action, value);
    
    protected virtual void Reset()
    {
        if (configuration == null)
            configuration = new ToggleConfiguration<T>();

        configuration.Validate(transform);
    }

    // Cannot use as MenuItem, because log warning: .SimulateClick is generic and cannot be used for menu commands.
    protected virtual void SimulateClick() => configuration.SimulateClick();

    // Cannot use as MenuItem, because: .ForceValidate is generic and cannot be used for menu commands.
    protected virtual void ForceValidate() => configuration.Validate(transform);
}

[Serializable]
public sealed class ToggleConfiguration<T> where T : struct, Enum
{
    [SerializeField] private Toggle _toggle;
    [SerializeField] private T _action;

    public T ActionType => _action;
    
    public Toggle Toggle => _toggle;
    
    public event Action<T, bool> OnClickToggle;

    public void Observe() => _toggle.onValueChanged.AddListener(ToggleClick);

    internal void Forget() => _toggle.onValueChanged.RemoveListener(ToggleClick);

    private void ToggleClick(bool value) => OnClickToggle?.Invoke(_action, value);
    
    public void Validate(Transform root) => this.With(x => x._toggle = root.GetComponent<Toggle>(), _toggle == null);

    public void SimulateClick()
    {
        Log.Info($"Invoke editor simulation toggle: {!_toggle.isOn}");
        _toggle.isOn = !_toggle.isOn;
    }
}
}