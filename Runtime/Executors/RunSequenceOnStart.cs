using System.Collections;
using UnityEngine;

namespace OrderedActionSequences.Executors
{
    internal sealed class RunSequenceOnStart : MonoBehaviour
    {
        [SerializeField]
        private OrderedActionSequenceBehaviour _sequence;

        private void Start()
        {
            StartCoroutine(_sequence.RunSequence());
        }
    }
}
