using System.Collections;
using UnityEngine;

namespace OrderedActionSequences
{
    public sealed class WaitForCompletionSource 
    {
        private readonly ICompletionSource _src;

        private WaitForCompletionSource(ICompletionSource src)
        {
            _src = src;
        }

        public static IEnumerator Wait(ICompletionSource src)
        {
            yield return new WaitForCompletionSource(src).Wait();
        }

        private IEnumerator Wait()
        {
            return new WaitUntil(() =>
            {
                return _src.IsComplete;
            });
        }
    }
}
