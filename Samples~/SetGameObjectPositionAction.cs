using OrderedActionSequences;
using UnityEngine;

internal sealed class SetGameObjectPositionAction : MonoBehaviour, IActionSequence
{
    [SerializeField]
    private Vector3 _position;

    [SerializeField]
    private Transform _gameObject;

    public ICompletionSource OnEnd()
    {
        return CompletionSource.Completed;
    }

    public ICompletionSource OnStart()
    {
        _gameObject.SetPositionAndRotation(_position, _gameObject.rotation);
        return CompletionSource.Completed;
    }
}