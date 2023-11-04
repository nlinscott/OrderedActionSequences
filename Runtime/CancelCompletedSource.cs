namespace OrderedActionSequences
{
    internal sealed class CancelCompletedSource : CompletionSource, ICancellationSource, ICancelToken
    {
        private static readonly ICancelToken _neverCancelled = new CancelCompletedSource();

        public static ICancelToken NeverCancelled
        {
            get
            {
                return _neverCancelled;
            }
        }

        public bool IsCancelled
        {
            get;
            private set;
        }

        public void Cancel()
        {
            IsCancelled = true;
        }
    }
}
