using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Game
{
[System.Diagnostics.DebuggerNonUserCode]
public static class Log
{
    public static bool enable = true;
    public static bool enableAnalyticsEvents = false;
    private static string InfoType => "[info]";
    private static string WarningType => "[warning]";
    private static string ErrorType => "[error]";
    private static string ExceptionType => "[exception]";
    private static string DebugType => "[debug]";
    private static string CriticalType => "[critical]";
    private static string AnalyticsType => "[analytics]";

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [HideInCallstack]
    public static void Write(string text, Object context = null)
    {
#if !DISABLE_LOG
        InternalLog(text, context);
#endif
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [HideInCallstack]
    public static void Info(string text, Object context = null)
    {
#if !DISABLE_LOG
        InternalLog($"{ColoredLogType(InfoType, Green)} " + text, context);
#endif
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [HideInCallstack]
    public static void Warning(string text, Object context = null)
    {
#if !DISABLE_LOG
        InternalLog($"{ColoredLogType(WarningType, Orange)} " + text, context);
#endif
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [HideInCallstack]
    public static void Error(string text, Object context = null)
    {
#if !DISABLE_LOG
        InternalLog($"{ColoredLogType(ErrorType, Red)} " + text, context);
#endif
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [HideInCallstack]
    public static void Exception(string text, Object context = null)
    {
#if !DISABLE_LOG
        InternalLog($"{ColoredLogType(ExceptionType, Pink)} " + text, context);
#endif
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [HideInCallstack]
    public static void InternalError(Object context = null)
    {
#if !DISABLE_LOG
        InternalLog($"{ColoredLogType(CriticalType, Blue)} " + "Internal error", context);
#endif
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [HideInCallstack]
    public static void Analytics(object action, Object context = null)
    {
#if !DISABLE_LOG
        if (enableAnalyticsEvents == false)
            return;
        InternalLog($"{ColoredLogType(AnalyticsType, Yellow)} " +
                    $"Event={action.GetType().Name};" +
                    $"Value={string.Join(";", action.GetType().GetFields().ToDictionary(x => x.Name, x => x.GetValue(action)))}",
                    context);
#endif
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [HideInCallstack]
    private static void InternalLog(string text, Object context)
    {
#if !DISABLE_LOG
        if (enable == false)
            return;
        Debug.Log(text, context);
#endif
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [HideInCallstack]
    private static string ColoredLogType(string type, string color)
    {
        return !Application.isEditor || Application.isBatchMode
            ? string.Concat($"[{DynamicData.GameData.Identifier}]", type)
            : string.Format(color, type);
    }

    #region Colors

    private static string NonColor => "{0}";
    private static string Green => "<color=#00FF00>{0}</color>";
    private static string Orange => "<color=#FF8000>{0}</color>";
    private static string Red => "<color=#EF4E4E>{0}</color>";
    private static string Blue => "<color=#0000FF>{0}</color>";
    private static string Cyan => "<color=#00FFFF>{0}</color>";
    private static string Yellow => "<color=#FFFF00>{0}</color>";
    private static string Violet => "<color=#8F00FF>{0}</color>";
    private static string Pink => "<color=#FFC0CB>{0}</color>";

    #endregion
}
}