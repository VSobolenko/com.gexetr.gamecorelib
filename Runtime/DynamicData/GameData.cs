using UnityEngine;

namespace Game.DynamicData
{
internal static class GameData
{
    internal const string EditorName = "Tools/GCL";
    internal static string Identifier => Application.identifier;
}
}