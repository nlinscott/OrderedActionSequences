using OrderedActionSequences.ActionSequences.Decorator.Base;
using OrderedActionSequences.Model;
using System.Collections;
using UnityEngine;

namespace OrderedActionSequences.ActionSequences.Decorator.Base
{
    public abstract class RepeatingActionSequenceBase<T> : RepeatingSequenceBase where T : RepeatingOrderedActionSequence
    {
        protected T SequenceData
        {
            get;
            private set;
        }

        private CompletionSource _src;

        protected override void Awake()
        {
            base.Awake();

            SequenceData = GetComponent<T>();

            if(SequenceData == null)
            {
                Debug.Log($"sequence data for {this.gameObject.name} is missing!");
            }
        }

        protected abstract IEnumerator RunRepetitions();

        private IEnumerator WaitToRepeat()
        {
            if (SequenceData.RepeatIntervalSeconds > 0)
            {
                yield return new WaitForSeconds(SequenceData.RepeatIntervalSeconds);
            }
        }

        protected IEnumerator RunOnStart()
        {
            ICompletionSource started = _actiontoRepeat.OnStart();

            yield return WaitForCompletionSource.Wait(started);
        }

        protected IEnumerator RunOnEnd()
        {
            ICompletionSource ended = _actiontoRepeat.OnEnd();

            yield return WaitForCompletionSource.Wait(ended);

            yield return WaitToRepeat();
        }

        protected IEnumerator RunActionToEnd()
        {
            yield return _actiontoRepeat.RunToEnd();

            yield return WaitToRepeat();
        }

        private IEnumerator RunToCompletion()
        {
            yield return RunRepetitions();

            _src.MarkCompleted();
        }

        public override ICompletionSource OnEnd()
        {
            return CompletionSource.Completed;
        }

        public override ICompletionSource OnStart()
        {
            if (_src == null || (_src != null && _src.IsComplete))
            {
                //we either havent been started yet or we ran to completion. Start again.
                _src = new CompletionSource();

                StartCoroutine(RunToCompletion());
            }

            //if !_src.IsComplete, do not overwrite existing completion source.
            return _src;
        }
    }
}
