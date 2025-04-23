using System.Collections;
using UnityEngine;

namespace OrderedActionSequences.Executors
{
    public sealed class SyncOrderedActionSequenceBehaviour : MonoBehaviour
    {
        private SynchronousSequenceExecutor _executor;

        private ISequenceDataExtractor _sequences;

        private void Awake()
        {
            EnsureInitialized();
        }

        public void RunSequence()
        {
            EnsureInitialized();
            _executor.Execute();
        }

        private void EnsureInitialized()
        {
            if (_sequences == null || _executor == null)
            {
                _sequences = new SequenceDataExtractor(this.transform);
                _executor = new SynchronousSequenceExecutor(_sequences);
            }
        }
    }
}
