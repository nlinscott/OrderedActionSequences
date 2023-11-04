using UnityEngine;

namespace OrderedActionSequences.Model
{
    class SyncedSequence : SequenceDataBase
    {
        [SerializeField]
        private long _targetOrderID;

        public long TargetOrderID
        {
            get
            {
                return _targetOrderID;
            }
        }
    }
}
