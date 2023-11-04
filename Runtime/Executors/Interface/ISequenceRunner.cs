using System.Collections;

namespace OrderedActionSequences.Executors
{
    interface ISequenceRunner
    {
        ICompletionSource Run(SequenceItem item);
    }
}
