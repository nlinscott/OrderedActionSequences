using OrderedActionSequences.ActionSequences.Decorator;
using OrderedActionSequences.ActionSequences.Decorator.Base;
using OrderedActionSequences.Model;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace OrderedActionSequences
{
    internal sealed class SequenceDataExtractor : ISequenceDataExtractor
    {
        public ILookup<long, SequenceItem> ActionSequences
        {
            get;
            private set;
        }

        public ILookup<long, SequenceItem> RepeatingSequences
        {
            get;
            private set;
        }

        public ILookup<long, SyncedSequenceItem> SyncedSequences
        {
            get;
            private set;
        }

        public IEnumerable<long> DistinctSequences
        {
            get;
            private set;
        }

        public SequenceDataExtractor(Transform parent)
        {
            InitializeSequences(parent);
        }

        private void LogMissingSequence(Transform transform)
        {
            Debug.LogError($"{transform.name} is missing an action sequence.");
        }

        private (T, U) ExtractSequenceData<T, U>(Transform transform)
            where T : IOrderedSequence
            where U : IActionSequence

        {
            T data = transform.gameObject.GetComponent<T>();

            if (data == null)
            {
                return default((T, U));
            }

            U sequence = transform.gameObject.GetComponent<U>();

            if (sequence == null)
            {
                LogMissingSequence(transform);
                return default((T, U));
            }

            return (data, sequence);
        }

        private bool TryGetSequence<T, U>(Transform transform, out SequenceItem item)
            where T : IOrderedSequence
            where U : IActionSequence
        {
            item = null;

            (T, U) result = ExtractSequenceData<T, U>(transform);

            if (result.Equals(default((T, U))))
            {
                return false;
            }

            item = new SequenceItem(result.Item1, result.Item2);

            return true;
        }

        private bool TryGetSyncedSequence<U>(Transform transform, out SyncedSequenceItem item)
           where U : IActionSequence
        {
            item = null;

            (SyncedSequence, U) result = ExtractSequenceData<SyncedSequence, U>(transform);

            if (result.Equals(default((SyncedSequence, U))))
            {
                return false;
            }

            item = new SyncedSequenceItem(result.Item1, result.Item2, result.Item1.TargetOrderID);

            return true;
        }

        private void InitializeSequences(Transform parent)
        {
            List<SequenceItem> actionSequences = new List<SequenceItem>();
            List<SequenceItem> repeatingSequences = new List<SequenceItem>();
            List<SyncedSequenceItem> syncedSequences = new List<SyncedSequenceItem>();

            foreach (Transform child in parent)
            {
                SyncedSequenceItem syncedItem;
                if (TryGetSyncedSequence<TransactionalActionSequence>(child, out syncedItem))
                {
                    syncedSequences.Add(syncedItem);
                    continue;
                }

                SequenceItem item;

                if (TryGetSequence<RepeatingOrderedActionSequence, RepeatingSequenceBase>(child, out item))
                {
                    repeatingSequences.Add(item);
                }
                else if (TryGetSequence<OrderedActionSequence, IActionSequence>(child, out item))
                {
                    actionSequences.Add(item);
                }
            }

            ActionSequences = actionSequences.ToLookup(l => l.Data.OrderID, s => s);

            RepeatingSequences = repeatingSequences.ToLookup(l => l.Data.OrderID, s => s);

            SyncedSequences = syncedSequences.ToLookup(l => l.Data.OrderID, s => s);

            SetDistinctSequences();
        }

        private void SetDistinctSequences()
        {
            IEnumerable<long> keys = ActionSequences.Select(s => s.Key);

            IEnumerable<long> repKeys = RepeatingSequences.Select(s => s.Key);

            IEnumerable<long> syncKeys = SyncedSequences.Select(s => s.Key);

            IEnumerable<long> allKeys = keys.Concat(repKeys).Concat(syncKeys);

            DistinctSequences = allKeys.OrderBy(s => s).Distinct();
        }
    }
}
