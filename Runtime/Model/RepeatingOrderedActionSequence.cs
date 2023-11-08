using UnityEngine;

namespace OrderedActionSequences.Model
{
    public abstract class RepeatingOrderedActionSequence : SequenceDataBase
    {
        public float RepeatIntervalSeconds => _repeatIntervalSeconds;

        [SerializeField]
        private float _repeatIntervalSeconds;
    }
}
