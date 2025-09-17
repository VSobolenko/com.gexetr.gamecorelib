using System;
using System.Linq;

namespace Game.Extensions
{
public static class EnumExtensions
{
    public static TEnum ToEnum<TEnum>(this string value, TEnum defaultValue = default) where TEnum : struct, Enum
    {
        return Enum.TryParse(value, true, out TEnum result) ? result : defaultValue;
    }

    public static T Random<T>(this T _, params T[] exclude) where T : Enum
    {
        var values = Enum.GetValues(typeof(T)).Cast<T>().ToList();

        if (exclude != null && exclude.Length > 0)
            values.RemoveAll(v => exclude.Contains(v));

        if (values.Count == 0)
            throw new InvalidOperationException($"Not found {typeof(T).Name} free elements.");

        var index = UnityEngine.Random.Range(0, values.Count);

        return values[index];
    }

    public static T GetRandomValue<T>(params T[] exclude) where T : Enum
    {
        var values = Enum.GetValues(typeof(T)).Cast<T>().ToList();

        if (exclude != null && exclude.Length > 0)
            values.RemoveAll(exclude.Contains);

        if (values.Count == 0)
            throw new InvalidOperationException($"Not found {typeof(T).Name} free elements.");

        var index = UnityEngine.Random.Range(0, values.Count);

        return values[index];
    }
}
}