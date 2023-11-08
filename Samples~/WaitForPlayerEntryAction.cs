using OrderedActionSequences;
using System.Collections;
using UnityEngine;

internal sealed class WaitForPlayerEntryAction : MonoBehaviour, IActionSequence
{
    [SerializeField]
    private Transform _player;

    [SerializeField]
    private bool _disableCollider = false;

    [SerializeField]
    private Collider _triggerCollider;

    private bool _hasPlayerEntered = false;

    private void Start()
    {
        PlayerEnteredTrigger trigger = _triggerCollider.gameObject.AddComponent<PlayerEnteredTrigger>();
        trigger.Action = this;
    }

    public ICompletionSource OnEnd()
    {
        _hasPlayerEntered = false;

        return CompletionSource.Completed;
    }

    public ICompletionSource OnStart()
    {
        _triggerCollider.enabled = true;

        CompletionSource src = new CompletionSource();

        StartCoroutine(WaitForPlayer(src));

        return src;
    }

    private IEnumerator WaitForPlayer(ICompletionToken token)
    {
        yield return new WaitUntil(() =>
        {
            return _hasPlayerEntered;
        });

        token.MarkCompleted();

        if (_disableCollider)
        {
            _triggerCollider.enabled = false;
        }

        _hasPlayerEntered = false;
    }

    private void PlayerEntered()
    {
        _hasPlayerEntered = true;
    }

    private sealed class PlayerEnteredTrigger : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if(other.gameObject.tag == "Player")
            {
                Action.PlayerEntered();
            }
        }

        public WaitForPlayerEntryAction Action
        {
            get;
            set;
        }
    }
}