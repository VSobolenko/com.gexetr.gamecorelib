using UnityEngine;

namespace Game.DynamicData
{
internal static class GameData
{
    internal const string EditorName = "Life or Death";
    internal static string Identifier => Application.identifier;
}
}