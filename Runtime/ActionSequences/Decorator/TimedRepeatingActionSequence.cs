using OrderedActionSequences.ActionSequences.Decorator.Base;
using OrderedActionSequences.Model;
using System.Collections;
using UnityEngine;

namespace OrderedActionSequences.ActionSequences.Decorator
{
    public class TimedRepeatingActionSequence : RepeatingActionSequenceBase<TimedRepeatingSequence>
    {
        private int _repetitions = 0;

        private float _runTime = 0;

        protected override IEnumerator RunRepetitions()
        {
            while (_runTime < SequenceData.RepeatForSeconds || _repetitions < SequenceData.MinRepetitions)
            {
                if (_repetitions > SequenceData.MaxRepetitions)
                {
                    break;
                }

                yield return RunActionToEnd();

                _runTime += Time.unscaledDeltaTime;

                _repetitions++;
            }

            _repetitions = 0;
            _runTime = 0;
        }
    }
}
