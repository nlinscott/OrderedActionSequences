namespace OrderedActionSequences
{
    public interface ICompletionSource
    {
        bool IsComplete
        {
            get;
        }

        void Link(ICompletionSource linkedSource);
    }
}
