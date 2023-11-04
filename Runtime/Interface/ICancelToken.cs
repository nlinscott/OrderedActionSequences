namespace OrderedActionSequences
{
    internal interface ICancelToken : ICompletionToken
    {
        bool IsCancelled
        {
            get;
        }
    }
}
