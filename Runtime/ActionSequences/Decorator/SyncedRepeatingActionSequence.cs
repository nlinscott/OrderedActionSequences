using OrderedActionSequences.ActionSequences.Decorator.Base;

namespace OrderedActionSequences.ActionSequences.Decorator
{
    public sealed class SyncedRepeatingActionSequence : SyncedSequenceBase
    {
        public override ICompletionSource OnEnd()
        {
            return _actiontoRepeat.OnEnd();
        }

        public override ICompletionSource OnStart()
        {
            return _actiontoRepeat.OnStart();
        }
    }
}
