using OrderedActionSequences;
using System.Collections;

namespace OrderedActionSequences.ActionSequences
{
    internal static class ActionSequenceExtensionMethods
    {
        internal static IEnumerator RunToEnd(this IActionSequence sequence)
        {
            ICompletionSource startSrc = sequence.OnStart();

            yield return WaitForCompletionSource.Wait(startSrc);

            ICompletionSource endSrc = sequence.OnEnd();

            yield return WaitForCompletionSource.Wait(endSrc);
        }
    }
}