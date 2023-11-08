using System.Linq;
using UnityEngine;

namespace OrderedActionSequences.ActionSequences.Decorator.Base
{
    public abstract class SequenceBase : MonoBehaviour, IActionSequence
    {
        protected IActionSequence _actiontoRepeat;

        protected virtual void Awake()
        {
            _actiontoRepeat = GetComponents<IActionSequence>().Except(this.AsEnumerable<IActionSequence>()).FirstOrDefault();
        }

        public abstract ICompletionSource OnEnd();

        public abstract ICompletionSource OnStart();
    }
}
