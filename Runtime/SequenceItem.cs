namespace OrderedActionSequences
{
    internal class SequenceItem
    {
        public SequenceItem(IOrderedSequence data, IActionSequence action)
        {
            Data = data;
            Action = action;
        }

        public IOrderedSequence Data
        {
            get;
        }

        public IActionSequence Action
        {
            get;
        }
    }
}
