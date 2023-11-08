using System.Collections;

namespace OrderedActionSequences.Executors
{
    internal interface ISequenceRunner
    {
        ICompletionSource Run(SequenceItem item);
    }
}
