using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Game
{
[System.Diagnostics.DebuggerNonUserCode]
public class Log
{
    public static ILogger logger = Debug.unityLogger;
    public static bool enable = true;
    public static bool enableAnalyticsEvents = false;
    
    private static string InfoType => "info";
    private static string WarningType => "warning";
    private static string ErroredType => "error";
    private static string ExceptionType => "exception";
    private static string DebugType => "debug";
    private static string CriticalType => "critical";
    private static string AnalyticsType => "analytics";
    private static string MethodType => "method";
    private static string FieldsType => "fields";

    private Log() { }
    
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetStatic()
    {
        logger = Debug.unityLogger;
        enable = true;
        enableAnalyticsEvents = false;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [HideInCallstack]
    public static void Write(string text, Object context = null, LogType logType = LogType.Log)
    {
#if !DISABLE_LOG
        InternalLog(text, context, logType);
#endif
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [HideInCallstack]
    public static void Warning(string text, Object context = null, LogType logType = LogType.Warning)
    {
#if !DISABLE_LOG
        InternalLog(text, context, logType);
#endif
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [HideInCallstack]
    public static void Error(string text, Object context = null, LogType logType = LogType.Error)
    {
#if !DISABLE_LOG
        InternalLog(text, context, logType);
#endif
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [HideInCallstack]
    public static void Info(string text, Object context = null, LogType logType = LogType.Log)
    {
#if !DISABLE_LOG
        Marked(InfoType, text, Color.Green, context, logType);
#endif
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [HideInCallstack]
    public static void Warner(string text, Object context = null, LogType logType = LogType.Log)
    {
#if !DISABLE_LOG
        Marked(WarningType, text, Color.Orange, context, logType);
#endif
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [HideInCallstack]
    public static void Errored(string text, Object context = null, LogType logType = LogType.Log)
    {
#if !DISABLE_LOG
        Marked(ErroredType, text, Color.Red, context, logType);
#endif
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [HideInCallstack]
    public static void Exception(string text, Object context = null, LogType logType = LogType.Log)
    {
#if !DISABLE_LOG
        Marked(ExceptionType, text, Color.Pink, context, logType);
#endif
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [HideInCallstack]
    public static void InternalError(Object context = null, LogType logType = LogType.Log)
    {
#if !DISABLE_LOG
        Marked(CriticalType, "Internal error", Color.DeepSkyBlue, context, logType);
#endif
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [HideInCallstack]
    public static void Method(int shift = 1, Object context = null, LogType logType = LogType.Log)
    {
#if !DISABLE_LOG
        var stackTrace = new System.Diagnostics.StackTrace();
        var frame = stackTrace.GetFrame(shift);
        var methodName = frame.GetMethod().Name;
        Marked(MethodType, methodName, Color.Beige, context, logType);
#endif
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [HideInCallstack]
    public static void AnalyticsParams(object action, Object context = null, LogType logType = LogType.Log)
    {
#if !DISABLE_LOG
        if (enableAnalyticsEvents == false)
            return;
        InternalLog($"{ColoredLogType(AnalyticsType, Color.Yellow)} " +
                    $"Event={action.GetType().Name};" +
                    $"Value={string.Join(";", action.GetType().GetFields().ToDictionary(x => x.Name, x => x.GetValue(action)))}",
            context, logType);
#endif
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [HideInCallstack]
    public static void Analytics(object text, Object context = null, LogType logType = LogType.Log)
    {
#if !DISABLE_LOG
        if (enableAnalyticsEvents == false)
            return;
        Marked(AnalyticsType, text, Color.Yellow, context, logType);

#endif
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [HideInCallstack]
    public static void Fields(object readableObject, object prefix = null, BindingFlags flags = BindingFlags.Public | BindingFlags.Instance, Object context = null, LogType logType = LogType.Log)
    {
#if !DISABLE_LOG
        var readable = readableObject.GetType();
        var fields = readable.GetFields(flags);
        var text = string.Join("; ", fields.Select(f => $"{f.Name}={f.GetValue(readableObject)}"));
        Marked(FieldsType, prefix + text, Color.Turquoise, context, logType);
#endif
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [HideInCallstack]
    public static void MarkedType(System.Type marker, object text, Color color = Color.White, Object context = null, LogType logType = LogType.Log)
    {
#if !DISABLE_LOG
        Marked(marker.Name, text, color, context, logType);
#endif
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [HideInCallstack]
    public static void Marked(string marker, object text, Color color = Color.Cyan, Object context = null, LogType logType = LogType.Log)
    {
#if !DISABLE_LOG
        InternalLog($"{ColoredLogType(marker, color)} " + text, context, logType);
#endif
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [HideInCallstack]
    public static void Colored(object text, Color color = Color.Violet, Object context = null, LogType logType = LogType.Log)
    {
#if !DISABLE_LOG
        InternalLog($"{ColoredLogType(text.ToString(), color)} ", context, logType);
#endif
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [HideInCallstack]
    private static void InternalLog(string text, Object context, LogType logType)
    {
#if !DISABLE_LOG
        if (logger.logEnabled == false || enable == false)
            return;
        logger.Log(logType, (object)text, context);
#endif
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [HideInCallstack]
    private static string ColoredLogType(string type, Color color)
    {
        return !Application.isEditor || Application.isBatchMode
            ? string.Concat($"[{GameData.Identifier}]", type)
            : string.Format($"<color=#{(int)color:X6}>[{{0}}]</color>", type);
    }

    public enum Color
    {
        Green    = 0x00FF00,
        Orange   = 0xFF8000,
        Red      = 0xEF4E4E,
        Blue     = 0x0000FF,
        Cyan     = 0x00FFFF,
        Yellow   = 0xFFFF00,
        Violet   = 0x8F00FF,
        Pink     = 0xFFC0CB,
        
        Brown       = 0x8B4513,
        Gray        = 0x808080,
        LightGray   = 0xD3D3D3,
        White       = 0xFFFFFF,
        Black       = 0x000000,
        Lime        = 0x32CD32,
        Navy        = 0x000080,
        Teal        = 0x008080,
        Olive       = 0x808000,
        Gold        = 0xFFD700,
        Silver      = 0xC0C0C0,
        Maroon      = 0x800000,
        Coral       = 0xFF7F50,
        Indigo      = 0x4B0082,
        Turquoise   = 0x40E0D0,
        Beige       = 0xF5F5DC,
        Salmon      = 0xFA8072,
        Tomato      = 0xFF6347,
        DeepSkyBlue = 0x00BFFF,
        HotPink     = 0xFF69B4,
        Aqua        = 0x00FFFF,
        Chocolate   = 0xD2691E
    }
    
    private static Color32 ToColor32(Color tag)
    {
        int hex = (int)tag;
        byte r = (byte)((hex >> 16) & 0xFF);
        byte g = (byte)((hex >> 8) & 0xFF);
        byte b = (byte)(hex & 0xFF);
        return new Color32(r, g, b, 255);
    }
}
}