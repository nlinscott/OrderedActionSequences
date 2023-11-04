using OrderedActionSequences.ActionSequences.Decorator.Base;

namespace OrderedActionSequences.ActionSequences.Decorator
{
    sealed class TransactionalActionSequence : SyncedSequenceBase
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
