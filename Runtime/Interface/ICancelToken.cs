namespace OrderedActionSequences
{
    public interface ICancelToken : ICompletionToken
    {
        bool IsCancelled
        {
            get;
        }
    }
}
