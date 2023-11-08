namespace OrderedActionSequences
{
    public interface IOrderedSequence
    {
        public long OrderID
        {
            get;
        }

        public float StartDelaySeconds
        {
            get;
        }
    }
}
