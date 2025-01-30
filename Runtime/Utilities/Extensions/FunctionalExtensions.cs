using System;

namespace Game.Extensions
{
public static class FunctionalExtensions
{
    public static T With<T>(this T self, Action<T> set)
    {
        set.Invoke(self);

        return self;
    }

    public static T With<T>(this T self, Action<T> apply, bool when)
    {
        if (when)
            apply?.Invoke(self);

        return self;
    }
    
    public static T With<T>(this T self, Action<T> apply, Func<bool> condition)
    {
        if (condition())
            apply?.Invoke(self);

        return self;
    }
    
    public static T With<T>(this T self, Action<T> apply, Func<T, bool> condition)
    {
        if (condition(self))
            apply?.Invoke(self);

        return self;
    }
}
}