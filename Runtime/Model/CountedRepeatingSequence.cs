using UnityEngine;

namespace OrderedActionSequences.Model
{
    public class CountedRepeatingSequence : RepeatingOrderedActionSequence
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
