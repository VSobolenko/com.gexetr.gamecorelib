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
    
    public static T With<T>(this T self, Action<T> apply, Func<bool> when)
    {
        if (when())
            apply?.Invoke(self);

        return self;
    }
    
    public static T With<T>(this T self, Action<T> apply, Func<T, bool> when)
    {
        if (when(self))
            apply?.Invoke(self);

        return self;
    }
    
    public static T With<T>(this T self, Action set)
    {
        set.Invoke();

        return self;
    }

    public static T With<T>(this T self, Action apply, bool when)
    {
        if (when)
            apply?.Invoke();

        return self;
    }
    
    public static T With<T>(this T self, Action apply, Func<bool> when)
    {
        if (when())
            apply?.Invoke();

        return self;
    }
    
    public static T With<T>(this T self, Action apply, Func<T, bool> when)
    {
        if (when(self))
            apply?.Invoke();

        return self;
    }
    
    public static T With<T>(this T self, int iterations, Action<T> set)
    {
        for (var i = 0; i < iterations; i++)
            set?.Invoke(self);

        return self;
    }
    
    public static T With<T>(this T self, int iterations, Action<T> apply, bool when)
    {
        for (var i = 0; i < iterations; i++)
            if (when)
                apply?.Invoke(self);

        return self;
    }
    
    public static T With<T>(this T self, int iterations, Action<int, T> set)
    {
        for (var i = 0; i < iterations; i++)
            set?.Invoke(i, self);

        return self;
    }
    
    public static T With<T>(this T self, int iterations, Action<int,T> apply, bool when)
    {
        for (var i = 0; i < iterations; i++)
            if (when)
                apply?.Invoke(i, self);

        return self;
    }
}
}