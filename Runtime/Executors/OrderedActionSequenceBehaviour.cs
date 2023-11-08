using System.Collections;
using UnityEngine;

namespace OrderedActionSequences.Executors
{
    [RequireComponent(typeof(SequenceRunner))]
    public sealed class OrderedActionSequenceBehaviour : MonoBehaviour
    {
        private OrderedActionSequenceExecutor _executor;

        private ISequenceDataExtractor _sequences;

        private void Awake()
        {
            _sequences = new SequenceDataExtractor(this.transform);
            _executor = new OrderedActionSequenceExecutor(_sequences, GetComponent<ISequenceRunner>());
        }

        public IEnumerator RunSequence()
        {
            return _executor.Execute();
        }
    }
}
