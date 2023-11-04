using OrderedActionSequences.ActionSequences.Decorator.Base;

namespace OrderedActionSequences.ActionSequences.Decorator
{
    sealed class SyncedRepeatingActionSequence : SyncedSequenceBase
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
