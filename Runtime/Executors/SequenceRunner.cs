using System.Collections;
using UnityEngine;

namespace OrderedActionSequences.Executors
{
    internal sealed class SequenceRunner : MonoBehaviour, ISequenceRunner
    {
        public ICompletionSource Run(SequenceItem item)
        {
            CompletionSource src = new CompletionSource();

            StartCoroutine(RunImpl(item, src));

            return src;
        }

        private IEnumerator RunImpl(SequenceItem item, CompletionSource src)
        {
            if(item.Data.StartDelaySeconds > 0)
            {
                yield return new WaitForSeconds(item.Data.StartDelaySeconds);
            }

            ICompletionSource started = item.Action.OnStart();
            src.Link(started);

            src.MarkCompleted();
        }
    }
}
