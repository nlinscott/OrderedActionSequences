using OrderedActionSequences;
using System.Collections;
using UnityEngine;

internal sealed class WaitForTimeAction : MonoBehaviour, IActionSequence
{
    [SerializeField]
    private float _waitTime = 3f;

    public ICompletionSource OnEnd()
    {
        return CompletionSource.Completed;
    }

    public ICompletionSource OnStart()
    {
        CompletionSource src = new CompletionSource();

        StartCoroutine(Wait(src));

        return src;
    }

    private IEnumerator Wait(ICompletionToken token)
    {
        yield return new WaitForSecondsRealtime(_waitTime);
        token.MarkCompleted();
    }
}