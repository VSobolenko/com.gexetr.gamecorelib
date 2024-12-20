using System;
using UnityEngine;

namespace Game.Components.Utilities
{
/// <summary>
/// Class which provide inspector functionality
/// </summary>
internal class RaycastBypassEditorUI : MonoBehaviour
{
    private void Awake() => throw new ArgumentException($"\"{name}\" has editor only \"{GetType().Name}\" component.");
}
}