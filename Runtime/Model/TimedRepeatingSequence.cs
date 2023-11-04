using UnityEngine;

namespace OrderedActionSequences.Model
{
    class TimedRepeatingSequence : RepeatingOrderedActionSequence
    {
        [SerializeField]
        private float _repeatForSeconds;

        [SerializeField]
        private float _minRepetitions;

        [SerializeField]
        private float _maxRepetitions;

        public float RepeatForSeconds
        {
            get
            {
                return _repeatForSeconds;
            }
        }

        public float MinRepetitions
        {
            get
            {
                return _minRepetitions;
            }
        }

        public float MaxRepetitions
        {
            get
            {
                return _maxRepetitions;
            }
        }
    }
}
