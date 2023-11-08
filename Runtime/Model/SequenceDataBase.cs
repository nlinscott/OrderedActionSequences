using UnityEngine;

namespace OrderedActionSequences.Model
{
    public abstract class SequenceDataBase : MonoBehaviour, IOrderedSequence
    {
        public long OrderID => _orderID;

        public float StartDelaySeconds => _startDelaySeconds;

        [SerializeField]
        private long _orderID;

        [SerializeField]
        private float _startDelaySeconds;
    }
}
