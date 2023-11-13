using UnityEngine;

namespace OrderedActionSequences.Executors
{
    /// <summary>
    /// A behaviour to run an ordered action sequence. In many cases its preferred to get a reference to and execute it directly,
    /// but in some cases you may want an arbitrary executor in case the initial executor gets destroyed.
    /// 
    /// When running Coroutines, if the behaviour context where the coroutine was started is destroyed or disabled, the coroutine exectuion is stopped.
    /// 
    /// </summary>
    public sealed class SequenceRunBehaviour : MonoBehaviour
    {
        [SerializeField]
        private OrderedActionSequenceBehaviour _sequence;

        public void Run()
        {
            StartCoroutine(_sequence.RunSequence());
        }
    }
}
