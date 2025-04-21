using System;

namespace Game.Extensions
{
public static class EnumExtensions
{
    public static TEnum ToEnum<TEnum>(this string value, TEnum defaultValue = default) where TEnum : struct, Enum
    {
        return Enum.TryParse(value, true, out TEnum result) ? result : defaultValue;
    }
}
}