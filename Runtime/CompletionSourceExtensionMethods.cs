using System.Collections.Generic;
using System.Linq;

namespace OrderedActionSequences
{
    public static class CompletionSourceExtensionMethods
    {
        internal static IEnumerable<ICompletionSource> StartAll(this IEnumerable<IActionSequence> sequences)
        {
            return sequences.Select(s => s.OnStart()).ToList();
        }

        internal static IEnumerable<ICompletionSource> EndAll(this IEnumerable<IActionSequence> sequences)
        {
            return sequences.Select(s => s.OnEnd()).ToList();
        }

        internal static void LinkAll(this ICompletionSource start, params ICompletionSource[] sources)
        {
            LinkAll(start, sources.ToList());
        }

        internal static void LinkAll(this ICompletionSource start, IEnumerable<ICompletionSource> others)
        {
            int itemsCount = others.Count();

            int i = 0;
            while (i < itemsCount - 1)
            {
                ICompletionSource item = others.ElementAt(i);

                ICompletionSource unlinked = item.FindUnlinkedItem();

                ICompletionSource itemToLink = others.ElementAt(++i);

                unlinked.Link(itemToLink);
            }

            if (itemsCount != 0)
            {
                start.Link(others.First());
            }
        }

        private static ICompletionSource FindUnlinkedItem(this ICompletionSource source)
        {
            ICompletionSource parent = source;
            ICompletionSource linked = parent.GetLinked();

            while (linked != null)
            {
                parent = linked;
                linked = parent.GetLinked();
            }

            return parent;
        }
    }
}
