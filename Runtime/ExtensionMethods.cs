using System.Collections.Generic;

internal static class ExtensionMethods
{
    internal static IEnumerable<T> AsEnumerable<T>(this T item)
    {
        yield return item;
    }
}