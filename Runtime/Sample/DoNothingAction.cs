using UnityEngine;

namespace OrderedActionSequences
{
    internal sealed class DoNothingAction : MonoBehaviour, IActionSequence
    {
        public ICompletionSource OnEnd()
        {
            return CompletionSource.Completed;
        }

        public ICompletionSource OnStart()
        {
            return CompletionSource.Completed;
        }
    }
}
