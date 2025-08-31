using System;
using Game.Extensions;
using UnityEngine;
using UnityEngine.UI;

namespace Game.GUI.Windows.Components
{
public class BaseToggle<T> : MonoBehaviour where T : struct, Enum
{
    [SerializeField] private ToggleConfiguration<T> _configuration;
    
    public event Action<T, bool> OnClickToggle;

    private void Start()
    {
        _configuration.ObserveToggle();
        _configuration.OnClickToggle += ClickToggle;
    }

    private void ClickToggle(T action, bool value) => OnClickToggle?.Invoke(action, value);
    private void OnValidate() => _configuration?.ValidateToggle(transform);
}

[Serializable,]
internal sealed class ToggleConfiguration<T> where T : struct, Enum
{
    [SerializeField] private Toggle _toggle;
    [SerializeField] private T _action;

    public event Action<T, bool> OnClickToggle;

    private void ToggleClick(bool value) => OnClickToggle?.Invoke(_action, value);
   
    public void ObserveToggle() => _toggle.onValueChanged.AddListener(ToggleClick);

    public void ValidateToggle(Transform root) =>
        this.With(x => x._toggle = root.GetComponent<Toggle>(), _toggle == null);
}
}