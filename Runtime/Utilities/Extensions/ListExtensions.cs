using System;
using System.Collections.Generic;

namespace Game.Extensions
{
public static class ListExtensions
{
    public static IList<T> Shuffle<T>(this IList<T> list)
    {
        var random = new Random();
        var count = list.Count;
        while (count > 1)
        {
            count--;
            var k = random.Next(count + 1);
            var value = list[k];
            list[k] = list[count];
            list[count] = value;
        }

        return list;
    }

    public static T[] Shuffle<T>(this T[] array)
    {
        var random = new Random();
        var count = array.Length;
        while (count > 1)
        {
            count--;
            var k = random.Next(count + 1);
            var value = array[k];
            array[k] = array[count];
            array[count] = value;
        }

        return array;
    }
    
    public static IList<T> Shuffle<T>(this IList<T> list, Random randomInstance)
    {
        var count = list.Count;
        while (count > 1)
        {
            count--;
            var k = randomInstance.Next(count + 1);
            var value = list[k];
            list[k] = list[count];
            list[count] = value;
        }

        return list;
    }

    public static T Random<T>(this IList<T> collection, int minIndex = 0, int maxIndex = -1) =>
        collection[UnityEngine.Random.Range(minIndex, maxIndex == -1 ? collection.Count : maxIndex)];
}
}