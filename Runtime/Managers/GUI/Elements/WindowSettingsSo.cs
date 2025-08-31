using System;
using DG.Tweening;
using Game.DynamicData;
using UnityEngine;

namespace Game.GUI.Windows
{
[Serializable]
public sealed class WindowSettings
{
    [SerializeField] private float _moveDuration = .5f;
    [SerializeField] private float _fadeDuration = .5f;
    [SerializeField, Range(0.1f, 10), Min(0.1f)] private float _synchronicity = 1; // use not for all transition
    [SerializeField] private Ease _openType = Ease.Linear;
    [SerializeField] private Ease _closeType = Ease.Linear;
    [SerializeField] internal TransitionSettings _bouncedOpen;
    [SerializeField] internal TransitionSettings _bouncedClose;

    public float MoveDuration => _moveDuration;
    public float FadeDuration => _fadeDuration;
    public float Synchronicity => _synchronicity;
    public Ease OpenType => _openType;
    public Ease CloseType => _closeType;
}

[CreateAssetMenu(fileName = nameof(WindowSettings), menuName = GameData.EditorName + "/Window Settings", order = 3)]
internal sealed class WindowSettingsSo : ScriptableObject
{
    [Header("Default Transition")]
    public WindowSettings _defaultSettings;
}

[Serializable]
internal struct TransitionSettings
{
    public float _duration;
    public Ease _ease;
}
}