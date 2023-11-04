namespace OrderedActionSequences
{
    internal class SyncedSequenceItem : SequenceItem
    {
        public long TargetID
        {
            get;
        }

        public SyncedSequenceItem(IOrderedSequence data, IActionSequence action, long targetID) 
            : base(data, action)
        {
            TargetID = targetID;
        }
    }
}
