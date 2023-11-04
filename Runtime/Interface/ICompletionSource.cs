namespace OrderedActionSequences
{
    internal interface ICompletionSource
    {
        bool IsComplete
        {
            get;
        }

        void Link(ICompletionSource linkedSource);
    }
}
