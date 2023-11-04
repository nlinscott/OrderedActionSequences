using System.Collections.Generic;
using System.Linq;

namespace OrderedActionSequences
{
    interface ISequenceDataExtractor
    {
        ILookup<long, SequenceItem> ActionSequences { get; }

        IEnumerable<long> DistinctSequences { get; }

        ILookup<long, SequenceItem> RepeatingSequences { get; }

        ILookup<long, SyncedSequenceItem> SyncedSequences { get; }
    }
}