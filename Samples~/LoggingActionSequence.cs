using OrderedActionSequences;
using UnityEngine;

internal sealed class LoggingActionSequence : MonoBehaviour, IActionSequence
{
    public ICompletionSource OnEnd()
    {
        Debug.Log("Logging Action Sequence - Done!");
        return CompletionSource.Completed;
    }

    public ICompletionSource OnStart()
    {
        Debug.Log("Logging Action Sequence - Begin...");
        return CompletionSource.Completed;
    }
}