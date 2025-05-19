using UnityEngine;

namespace Game.GUI.Windows
{
public interface IWindowListener
{
    void OnInitialize();
    void OnFocus();
    void OnUnfocused();
    void OnDestroy();
}

public interface IMediator : IWindowListener
{
    void SetActive(bool value);
    void SetPosition(Vector3 value);
    void SetInteraction(bool value);
    bool IsActive();
    void Destroy();
}
}