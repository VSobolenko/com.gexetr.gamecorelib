using System.Collections.Generic;

namespace Game.Extensions
{
public static class ListExtensions
{
    public static IList<T> Shuffle<T>(this IList<T> list)
    {
        int count = list.Count;
        while (count > 1)
        {
            count--;
            int k = UnityEngine.Random.Range(0, count + 1);
            T value = list[k];
            list[k] = list[count];
            list[count] = value;
        }

        return list;
    }

    public static T[] Shuffle<T>(this T[] array)
    {
        int count = array.Length;
        while (count > 1)
        {
            count--;
            int k = UnityEngine.Random.Range(0, count + 1);
            T value = array[k];
            array[k] = array[count];
            array[count] = value;
        }

        return array;
    }

    public static IList<T> Shuffle<T>(this IList<T> list, System.Random randomInstance)
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

    public static bool IsNullOrEmpty<T>(this IList<T> collection) => collection == null || collection.Count == 0;

    public static void InsertAtRandomPosition<T>(this List<T> list, T item)
    {
        if (list == null)
            throw new System.ArgumentNullException(nameof(list));

        var index = UnityEngine.Random.Range(0, list.Count + 1);
        list.Insert(index, item);
    }
}
}