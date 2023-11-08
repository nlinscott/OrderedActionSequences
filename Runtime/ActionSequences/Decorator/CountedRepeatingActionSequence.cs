using OrderedActionSequences.ActionSequences.Decorator.Base;
using OrderedActionSequences.Model;
using System.Collections;

namespace OrderedActionSequences.ActionSequences.Decorator
{
    public class CountedRepeatingActionSequence : RepeatingActionSequenceBase<CountedRepeatingSequence>
    {
        private int _count = 0;

        protected override IEnumerator RunRepetitions()
        {
            do
            {
                yield return RunActionToEnd();
                _count++;
            }
            while (_count < SequenceData.RepeatCount);

            _count = 0;
        }
    }
}
