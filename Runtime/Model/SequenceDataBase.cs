using UnityEngine;

namespace OrderedActionSequences.Model
{
    abstract class SequenceDataBase : MonoBehaviour, IOrderedSequence
    {
        public long OrderID => _orderID;

        public float StartDelaySeconds => _startDelaySeconds;

        [SerializeField]
        private long _orderID;

        [SerializeField]
        private float _startDelaySeconds;
    }
}
