using UnityEngine;

namespace OrderedActionSequences.Model
{
    class CountedRepeatingSequence : RepeatingOrderedActionSequence
    {
        [SerializeField]
        private int _repeatCount;

        public int RepeatCount
        {
            get
            {
                return _repeatCount;
            }
        }
    }
}
