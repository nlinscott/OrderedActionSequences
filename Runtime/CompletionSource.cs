namespace OrderedActionSequences
{
    internal class CompletionSource : ICompletionSource, ICompletionToken
    {
        public static ICompletionSource Completed
        {
            get
            {
                CompletionSource src = new CompletionSource();
                src.MarkCompleted();
                return src;
            }
        }

        private bool _isComplete = false;

        public bool IsComplete
        {
            get
            {
                if(_linkedSource != null)
                {
                    return _isComplete && _linkedSource.IsComplete;
                }

                return _isComplete;
            }
        }

        private ICompletionSource _linkedSource;

        public void Link(ICompletionSource linkedSource)
        {
            _linkedSource = linkedSource;
        }

        public void MarkCompleted()
        {
            _isComplete = true;
        }
    }
}
