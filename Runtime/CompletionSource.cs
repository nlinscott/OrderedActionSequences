using System;

namespace OrderedActionSequences
{
    public class CompletionSource : ICompletionSource, ICompletionToken
    {
#if UNITY_EDITOR

        private readonly string _id = Guid.NewGuid().ToString();
#endif
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
                if (_linkedSource != null)
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

        public ICompletionSource GetLinked()
        {
            return _linkedSource;
        }

        public void MarkCompleted()
        {
            _isComplete = true;
        }
    }
}
