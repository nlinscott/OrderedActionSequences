using System.Collections;
using System;
using UnityEngine;

namespace OrderedActionSequences.Executors
{
    internal sealed class SequenceRunner : MonoBehaviour, ISequenceRunner
    {
        public ICompletionSource Run(SequenceItem item)
        {
            CompletionSource src = new CompletionSource();

            Action run = () =>
            {
                ICompletionSource started = item.Action.OnStart();
                src.Link(started);

                src.MarkCompleted();
            };

            if (item.Data.StartDelaySeconds <= 0)
            {
                run();
            }
            else
            {
                StartCoroutine(RunWithDelay(item.Data.StartDelaySeconds, run));
            }

            return src;
        }

        private IEnumerator RunWithDelay(float delay, Action run)
        {
            yield return new WaitForSeconds(delay);

            run();
        }
    }
}
