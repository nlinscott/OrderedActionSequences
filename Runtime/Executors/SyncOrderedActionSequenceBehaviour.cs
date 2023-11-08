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
            _sequences = new SequenceDataExtractor(this.transform);
            _executor = new SynchronousSequenceExecutor(_sequences);
        }

        public void RunSequence()
        {
            _executor.Execute();
        }
    }
}
