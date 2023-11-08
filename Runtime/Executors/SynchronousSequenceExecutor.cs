using OrderedActionSequences;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace OrderedActionSequences.Executors
{
    internal sealed class SynchronousSequenceExecutor
    {
        private readonly ISequenceDataExtractor _sequences;

        public SynchronousSequenceExecutor(ISequenceDataExtractor sequences)
        {
            _sequences = sequences;
        }

        public void Execute()
        {
            if (!_sequences.DistinctSequences.Any())
            {
                Debug.Log($"Synchronous Ordered Action Sequence has no items to run.");
                return;
            }

            if (_sequences.SyncedSequences.Any())
            {
                Debug.Log($"Synchronous Ordered Action Sequence cannot have synced sequences as they will all be run synchronously.");
                return;
            }

            if (_sequences.RepeatingSequences.Any())
            {
                Debug.Log($"Synchronous Ordered Action Sequence cannot have repeating sequences as they will all be run synchronously.");
                return;
            }

            foreach (long orderID in _sequences.DistinctSequences)
            {
                IEnumerable<SequenceItem> seq = _sequences.ActionSequences[orderID];

                foreach (SequenceItem seqItem in seq)
                {
                    VerifyCompletion(seqItem.Action.OnStart(), seqItem);

                    VerifyCompletion(seqItem.Action.OnEnd(), seqItem);
                }
            }
        }

        private void VerifyCompletion(ICompletionSource src, SequenceItem item)
        {
            if (src.IsComplete)
            {
                return;
            }

            Debug.LogWarning($"{item.Action.GetType().Name} has not completed. Order ID: {item.Data.OrderID}");
        }
    }
}