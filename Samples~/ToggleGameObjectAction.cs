using OrderedActionSequences;
using UnityEngine;

internal sealed class ToggleGameObjectAction : MonoBehaviour, IActionSequence
{
    [SerializeField]
    private GameObject _subject;

    [SerializeField]
    private bool _enabled = false;

    public ICompletionSource OnEnd()
    {
        return CompletionSource.Completed;
    }

    public ICompletionSource OnStart()
    {
        _subject.SetActive(_enabled);
        return CompletionSource.Completed;
    }
}
