using System.Collections.Generic;

namespace OrderedActionSequences.Model
{
    internal sealed class SequenceDataComparer : IEqualityComparer<IOrderedSequence>
    {
        public bool Equals(IOrderedSequence x, IOrderedSequence y)
        {
            if(x == null && y == null)
            {
                return true;
            }

            if(x == null || y == null)
            {
                return false;
            }

            return x.OrderID == y.OrderID;
        }

        public int GetHashCode(IOrderedSequence obj)
        {
            return obj.OrderID.GetHashCode();
        }
    }
}
