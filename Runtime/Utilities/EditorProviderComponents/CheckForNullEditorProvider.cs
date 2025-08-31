using System;
using UnityEngine;

namespace Game.Components.Utilities
{
/// <summary>
/// Class which provide inspector functionality
/// </summary>
public sealed class CheckForNullEditorProvider : MonoBehaviour
{
    [Header("If empty check all"), SerializeField]
    internal string[] _accessible = { "Assembly-CSharp", "GameCoreLib" };

    [SerializeField] internal bool _enableClassPath;
    [SerializeField] internal MonoBehaviour[] _monoBehaviour;

    private void Awake() => throw new ArgumentException($"\"{name}\" has editor only \"{GetType().Name}\" component.");
}
}