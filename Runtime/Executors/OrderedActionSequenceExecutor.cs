using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;

namespace OrderedActionSequences.Executors
{
    internal sealed class OrderedActionSequenceExecutor
    {
        private readonly ISequenceDataExtractor _sequences;
        private readonly ISequenceRunner _runner;

        private readonly IDictionary<ICompletionSource, SequenceItem> _repeatingSequences = new Dictionary<ICompletionSource, SequenceItem>();
        private readonly IDictionary<ICompletionSource, SyncedSequenceItem> _syncedSequences = new Dictionary<ICompletionSource, SyncedSequenceItem>();

        public OrderedActionSequenceExecutor(ISequenceDataExtractor sequences, ISequenceRunner runner)
        {
            _sequences = sequences;
            _runner = runner;
        }

        public IEnumerator Execute()
        {
            if(!_sequences.DistinctSequences.Any())
            {
                Debug.Log($"Ordered Action Sequence has no items to run.");
            }

            foreach (long orderID in _sequences.DistinctSequences)
            {
                IEnumerable<SequenceItem> seq = _sequences.ActionSequences[orderID];

                IEnumerable<SequenceItem> repSeq = _sequences.RepeatingSequences[orderID];

                IEnumerable<SyncedSequenceItem> syncedSequences = _sequences.SyncedSequences[orderID];

                ICompletionSource currentSource = StartPrimarySequence(_runner, seq);

                StartSequences(_runner, repSeq, _repeatingSequences);
                StartSequences(_runner, syncedSequences, _syncedSequences);

                yield return WaitForCompletionSource.Wait(currentSource);

                ICompletionSource currentSourceEnd = EndPrimarySequence(seq);

                yield return WaitForCompletionSource.Wait(currentSourceEnd);

                if (seq.Any())
                {
                    yield return SynchronizeSequencesOn(orderID);
                }

                (ICompletionSource, IEnumerable<long>) result = EndAndRemoveRepeating();

                yield return WaitForCompletionSource.Wait(result.Item1);

                yield return SynchronizeSequencesOn(result.Item2);
            }

            yield return WaitForAllRepeatingSequences();
        }


        private static ICompletionSource StartPrimarySequence(ISequenceRunner runner, IEnumerable<SequenceItem> sequences)
        {
            ICompletionSource src = CompletionSource.Completed;

            IEnumerable<ICompletionSource> actions = sequences.Select(s => runner.Run(s)).ToList();

            src.LinkAll(actions);

            return src;
        }

        private static ICompletionSource EndPrimarySequence(IEnumerable<SequenceItem> sequences)
        {
            ICompletionSource src = CompletionSource.Completed;

            IEnumerable<ICompletionSource> actions = sequences.Select(s => s.Action.OnEnd()).ToList();

            src.LinkAll(actions);

            return src;
        }

        private void StartSequences<T>(ISequenceRunner runner, IEnumerable<T> sequences, IDictionary<ICompletionSource, T> cache)
            where T : SequenceItem
        {
            foreach (T item in sequences)
            {
                ICompletionSource src = runner.Run(item);

                cache.Add(src, item);
            }
        }

        //returns the competion source and all distinct order IDs that are ending
        private (ICompletionSource, IEnumerable<long>) EndAndRemoveRepeating()
        {
            ICollection<ICompletionSource> ending = new Collection<ICompletionSource>();
            ICollection<ICompletionSource> completed = new Collection<ICompletionSource>();

            IEnumerable<KeyValuePair<ICompletionSource, SequenceItem>> kvps = _repeatingSequences.Where(s => s.Key.IsComplete);

            IEnumerable<long> orderIDsCompleted = kvps.Select(v => v.Value.Data.OrderID).Distinct();

            foreach (KeyValuePair<ICompletionSource, SequenceItem> kvp in kvps)
            {
                completed.Add(kvp.Key);

                ICompletionSource endAction = kvp.Value.Action.OnEnd();

                ending.Add(endAction);
            }

            ICompletionSource src = CompletionSource.Completed;

            src.LinkAll(ending);

            foreach (ICompletionSource toRemove in completed)
            {
                _repeatingSequences.Remove(toRemove);
            }

            return (src, orderIDsCompleted);
        }

        private IEnumerator WaitForAllRepeatingSequences()
        {
            while(_repeatingSequences.Any())
            {
                (ICompletionSource, IEnumerable<long>) result = EndAndRemoveRepeating();

                yield return WaitForCompletionSource.Wait(result.Item1);

                SynchronizeSequencesOn(result.Item2);
            }
        }

        private IEnumerator SynchronizeSequencesOn(IEnumerable<long> orderIDs)
        {
            foreach(long ID in orderIDs)
            {
                yield return SynchronizeSequencesOn(ID);
            }
        }

        private IEnumerator SynchronizeSequencesOn(long orderID)
        {
            IEnumerable<KeyValuePair<ICompletionSource, SyncedSequenceItem>> allWithID = _syncedSequences.Where(kvp => kvp.Value.TargetID == orderID).ToList();

            if (!allWithID.Any())
            {
                yield break;
            }

            IEnumerable<ICompletionSource> completion = allWithID.Select(k => k.Key).ToList();
            
            ICompletionSource completedStart = CompletionSource.Completed;
            completedStart.LinkAll(completion);

            yield return WaitForCompletionSource.Wait(completedStart);

            IEnumerable<ICompletionSource> actions = allWithID.Select(kvp => kvp.Value.Action.OnEnd()).ToList();

            ICompletionSource completedEnd = CompletionSource.Completed;
            completedEnd.LinkAll(actions);

            yield return WaitForCompletionSource.Wait(completedEnd);

            yield return SynchronizeSequencesOn(allWithID.Select(kvp => kvp.Value.Data.OrderID).ToList());

            foreach (ICompletionSource key in completion)
            {
                _syncedSequences.Remove(key);
            }
        }
    }
}
