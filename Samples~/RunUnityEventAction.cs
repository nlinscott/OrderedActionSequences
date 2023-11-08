using OrderedActionSequences;
using UnityEngine;
using UnityEngine.Events;

internal sealed class RunUnityEventAction : MonoBehaviour, IActionSequence
{
    [SerializeField]
    private UnityEvent _event;

    public ICompletionSource OnEnd()
    {
        return CompletionSource.Completed;
    }

    public ICompletionSource OnStart()
    {
        _event.Invoke();
        return CompletionSource.Completed;
    }
}